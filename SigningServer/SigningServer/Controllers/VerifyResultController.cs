using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProxyServer.Data;
using ProxyServer.Models;

namespace SigningServer.Controllers
{
    public class VerifyResultController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<VerifyResultController> _logger;

        public VerifyResultController(ILogger<VerifyResultController> logger, ApplicationDbContext context)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Accept()
        {
            _logger.Log(LogLevel.Information, "Displaying Verification result");
            string username = (string)TempData["signer"];
            ViewBag.Name = username;
            return View();
        }

        public IActionResult Deny()
        {
            return View();
        }
    }
}
