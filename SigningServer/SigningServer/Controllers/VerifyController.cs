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
            string digest    = (string)json["hashed"];
            string user      = (string)json["user"];
            string signature = (string)json["signature"];
            _logger.Log(LogLevel.Information, "Digest: " + digest);
            _logger.Log(LogLevel.Information, "User: " + user);
            _logger.Log(LogLevel.Information, "Signature: " + signature);

            UnicodeEncoding byte_converter = new UnicodeEncoding();
            byte[] digest_bytes = byte_converter.GetBytes(digest);
            byte[] username_bytes = byte_converter.GetBytes(user);

            byte[] to_be_signed = new byte[digest_bytes.Length + username_bytes.Length];
            Buffer.BlockCopy(digest_bytes, 0, to_be_signed, 0, digest_bytes.Length);
            Buffer.BlockCopy(username_bytes, 0, to_be_signed, digest_bytes.Length, username_bytes.Length);

            string new_signature = Convert.ToBase64String(RSA.SignData(to_be_signed, SHA256.Create()));
            _logger.Log(LogLevel.Information, "Signature from file: " + signature);
            _logger.Log(LogLevel.Information, "New Signature: " + new_signature);

            if (Equals(signature, new_signature))
            {
                _logger.Log(LogLevel.Information, "VERIFIED");
                TempData["signer"] = user;
                return "YES";
            }

            _logger.Log(LogLevel.Information, "NOT VERIFIED");
            return "NO";
        }
    }
}
