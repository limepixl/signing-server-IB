using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace ASPNET_Test.Controllers
{
    public class HomeController : Controller
    {
        // 
        // GET: /Home/
        public IActionResult Index()
        {
            return View();
        }

        // 
        // GET: /Home/Welcome/ 
        public IActionResult Welcome(string name, int numTimes = 1)
        {
            ViewData["Message"] = "Hello " + name;
            ViewData["NumTimes"] = numTimes;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}