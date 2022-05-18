using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using ProxyServer.Data;
using Newtonsoft.Json.Linq;
using System.Text;
using ProxyServer.Models;
using System.Buffers.Text;

namespace ProxyServer.Controllers
{
    public class VerifyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly RSACryptoServiceProvider RSA;

        public VerifyController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _context = context;
            _logger = logger;
            RSA = new RSACryptoServiceProvider();

            ProxyServer.Models.ServerKeyPair keypair = _context.ServerKeyPair.ToList()[0];
            string publicKey = keypair.PublicKeyOnly;
            string privatePublicKeys = keypair.PrivatePublicKeyPair;

            RSA.FromXmlString(privatePublicKeys);
        }

        public IActionResult Index()
        {
            return View();
        }

        // [Authorize]
        [HttpPost]
        [Route("/Verify/RequestVerification")]
        public string RequestVerification()
        {
            StreamReader bodyStream = new StreamReader(HttpContext.Request.Body);
            Task<string> bodyText = bodyStream.ReadToEndAsync();
            bodyText.Wait();

            string content = bodyText.Result;

            JObject json = JObject.Parse(content);
            string digest = (string)json["hashed"];
            string signature = (string)json["signature"];
            _logger.Log(LogLevel.Information, "Digest: " + digest);
            _logger.Log(LogLevel.Information, "Signature: " + signature);

            UnicodeEncoding byte_converter = new UnicodeEncoding();
            byte[] digest_bytes = byte_converter.GetBytes(digest);
            byte[] signed_digest = RSA.SignData(digest_bytes, SHA256.Create());

            string new_signature = Convert.ToBase64String(signed_digest);
            new_signature = new_signature.Remove(new_signature.Length - 1, 1);
            _logger.Log(LogLevel.Information, "Sub Signature: " + new_signature);

            if (signature.Contains(new_signature))
            {
                string decoded_signature = Encoding.UTF8.GetString(Convert.FromBase64String(signature));
                string decoded_signer_id = decoded_signature.Remove(0, signed_digest.Length-7);
                
                _logger.Log(LogLevel.Information, $"Signer ID: {decoded_signer_id}");
                var signatureStatement = _context.SignatureStatement.Where(s => s.UserId == decoded_signer_id && s.MessageDigest == digest).ToArray()[0];
                _logger.Log(LogLevel.Information, "Signature statement: " + signatureStatement.ToString());
                _logger.Log(LogLevel.Information, "VERIFIED");

                string statement = Newtonsoft.Json.JsonConvert.SerializeObject(signatureStatement);
                TempData["statement"] = statement;
                return statement;
            }

            _logger.Log(LogLevel.Information, "NOT VERIFIED");
            return "NO";
        }
    }
}
