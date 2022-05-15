using Microsoft.AspNetCore.Mvc;

namespace ProxyServer.Controllers
{
    public class SignController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
