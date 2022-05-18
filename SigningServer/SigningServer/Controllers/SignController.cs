using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using System.IO.Pipelines;
using ProxyServer.Data;
using System.Text;
using ProxyServer.Models;
using Newtonsoft.Json.Linq;

namespace ProxyServer.Controllers
{
    public class SignController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly RSACryptoServiceProvider RSA;

        public SignController(ILogger<HomeController> logger, ApplicationDbContext context)
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

        [HttpPost]
        [Route("/Sign/RequestSignature")]
        public string RequestSignature()
        {
            StreamReader bodyStream = new StreamReader(HttpContext.Request.Body);
            Task<string> bodyText = bodyStream.ReadToEndAsync();
            bodyText.Wait();

            string digest = bodyText.Result;
            string id = _context.Users.Where(user => user.UserName == User.Identity.Name).SingleOrDefault().Id;

            UnicodeEncoding byte_converter = new UnicodeEncoding();
            byte[] digest_bytes = byte_converter.GetBytes(digest);
            byte[] signed_digest = RSA.SignData(digest_bytes, SHA256.Create());

            byte[] ID_bytes = byte_converter.GetBytes(id);
            byte[] final_signature = new byte[signed_digest.Length + ID_bytes.Length];
            Buffer.BlockCopy(signed_digest, 0, final_signature, 0, signed_digest.Length);
            Buffer.BlockCopy(ID_bytes, 0, final_signature, signed_digest.Length, ID_bytes.Length);

            SignatureStatement statement = new SignatureStatement();
            statement.MessageDigest = digest;
            statement.UserId = id;
            statement.Username = User.Identity.Name;
            statement.SignedOn = DateTime.Now;
            _context.SignatureStatement.Add(statement);
            _context.SaveChanges();

            string base64signature = Convert.ToBase64String(final_signature);

            JObject json = JObject.Parse("{ \"signature\" : \"" + base64signature + "\", \"user\": \"" + id + "\"}");
            _logger.Log(LogLevel.Information, json.ToString());
            return json.ToString();
        }
    }
}
