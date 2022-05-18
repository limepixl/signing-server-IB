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

        public IActionResult Index()
        {
            string statement = (string)TempData["statement"];
            var deserialized = JsonConvert.DeserializeObject<SignatureStatement>(statement);
            _logger.Log(LogLevel.Information, statement);
            return View(deserialized);
        }
    }
}
