using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;

namespace ProxyServer.Controllers
{
    public class SignController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // [Authorize]
        [HttpPost]
        public string RequestSignature(string content) {

            /*

                dobiva string shto e hashot

                treba da go potpishe i vrati potpisot kako string (base64 oblik)
            
            */
            
            return "hahaahha";
        }
    }
}
