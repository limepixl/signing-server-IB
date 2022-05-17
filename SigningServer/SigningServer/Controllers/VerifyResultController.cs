using Microsoft.AspNetCore.Mvc;
using ProxyServer.Data;

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
            ViewBag.MainNav = statement;
            _logger.Log(LogLevel.Information, statement);
            return View();
        }
    }
}
