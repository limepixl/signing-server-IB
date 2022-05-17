using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using ProxyServer.Data;

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
        public string RequestVerification(string content) {
            
            /* 
                dobiva json object od oblik

                {
                    "hashed": hashot od fajlovite vnatre vo zipot (bez signature fileot)
                    "signature": contents na signature fajlot
                }

                treba da vrati dali se poklopuvaat
                kako tochno toa ke se dogg otposle

            */

            return "UBIEC";
        }
    }
}
