using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using ProxyServer.Data;
using Newtonsoft.Json.Linq;
using System.Text;
using ProxyServer.Models;

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
        public string RequestVerification()
        {
            StreamReader bodyStream = new StreamReader(HttpContext.Request.Body);
            Task<string> bodyText = bodyStream.ReadToEndAsync();
            bodyText.Wait();

            string content = bodyText.Result;

            JObject json = JObject.Parse(content);
            string digest = (string)json["hashed"];
            string signature = (string)json["signature"];
            // _logger.Log(LogLevel.Information, "Digest: " + digest);
            // _logger.Log(LogLevel.Information, "Signature: " + signature);

            string id = _context.Users.Where(user => user.UserName == User.Identity.Name).SingleOrDefault().Id;

            // TODO: treba nekako da zememe samo eden so IDto
            var signatureStatement = _context.SignatureStatement.Where(s => s.UserId == id && s.MessageDigest == digest).ToArray()[0];

            UnicodeEncoding byte_converter = new UnicodeEncoding();
            byte[] digest_bytes = byte_converter.GetBytes(digest);
            byte[] signed_digest = RSA.SignData(digest_bytes, SHA256.Create());

            byte[] ID_bytes = byte_converter.GetBytes(id);
            byte[] final_signature = new byte[signed_digest.Length + ID_bytes.Length];
            Buffer.BlockCopy(signed_digest, 0, final_signature, 0, signed_digest.Length);
            Buffer.BlockCopy(ID_bytes, 0, final_signature, signed_digest.Length, ID_bytes.Length);

            if (Equals(Convert.ToBase64String(final_signature), signature))
            {
                _logger.Log(LogLevel.Information, "VERIFIED");

                string statement = Newtonsoft.Json.JsonConvert.SerializeObject(signatureStatement);
                TempData["statement"] = statement;

                // GOMNOVO NE RABOTI CRKNI CRKNI
                RedirectToAction("Index", "VerifyResultController");
                return "VERIFIED";
            }
            else
            {
                _logger.Log(LogLevel.Information, "NOT VERIFIED");
                return "NOT VERIFIED";
            }
        }
    }
}
