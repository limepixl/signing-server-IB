using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;

namespace ProxyServer.Controllers
{
    public class VerifyController : Controller
    {
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
