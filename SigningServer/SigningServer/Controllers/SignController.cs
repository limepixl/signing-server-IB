using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using ProxyServer.Data;
using ProxyServer.Models;
using System.Security.Cryptography;
using System.Text;

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
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [Route("/Sign/RequestSignature")]
        public string RequestSignature()
        {
            bool has2fa = _context.Users.Where(user => user.UserName == User.Identity.Name).SingleOrDefault().TwoFactorEnabled;
            if(!has2fa)
            {
                return "NO_2FA";
            }

            StreamReader bodyStream = new StreamReader(HttpContext.Request.Body);
            Task<string> bodyText = bodyStream.ReadToEndAsync();
            bodyText.Wait();

            string digest = bodyText.Result;
            string id = _context.Users.Where(user => user.UserName == User.Identity.Name).SingleOrDefault().Id;
            string username = User.Identity.Name;

            UnicodeEncoding byte_converter = new UnicodeEncoding();
            byte[] digest_bytes = byte_converter.GetBytes(digest);
            byte[] username_bytes = byte_converter.GetBytes(username);

            byte[] to_be_signed = new byte[digest_bytes.Length + username_bytes.Length];
            Buffer.BlockCopy(digest_bytes, 0, to_be_signed, 0, digest_bytes.Length);
            Buffer.BlockCopy(username_bytes, 0, to_be_signed, digest_bytes.Length, username_bytes.Length);

            byte[] signature = RSA.SignData(to_be_signed, SHA256.Create());

            SignatureStatement statement = new SignatureStatement();
            statement.MessageDigest = digest;
            statement.UserId = id;
            statement.Username = username;
            statement.SignedOn = DateTime.Now;
            _context.SignatureStatement.Add(statement);
            _context.SaveChanges();

            string base64signature = Convert.ToBase64String(signature);

            JObject json = JObject.Parse("{ \"signature\" : \"" + base64signature + "\", \"user\": \"" + statement.Username + "\"}");
            _logger.Log(LogLevel.Information, json.ToString());
            return json.ToString();
        }
    }
}
