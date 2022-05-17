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
            
            return "hahaahha";
        }
    }
}
