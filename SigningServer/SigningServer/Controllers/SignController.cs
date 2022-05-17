using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using ProxyServer.Data;
using ProxyServer.Models;


namespace ProxyServer.Controllers
{
    public class SignController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly ILogger<HomeController> _logger;

        static Helpers helper = new Helpers();
        Boolean keys_generated = helper.KeyGeneration();

        public SignController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;

        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public string RequestSignature(string content) {

            /*

                dobiva string shto e hashot

                treba da go potpishe i vrati potpisot kako string (base64 oblik)
            
            */
            var UserId = _context.Users.Where(user => user.UserName == User.Identity.Name).SingleOrDefault().Id;
            var Username = User.Identity.Name;
            var Payload = String.Concat(content, UserId, Username);



            return "hahaahha";
        }
    }
}
