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
            
            return "UBIEC";
        }
    }
}
