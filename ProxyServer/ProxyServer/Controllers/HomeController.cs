using Microsoft.AspNetCore.Mvc;
using ProxyServer.Models;
using System.Diagnostics;
using ProxyServer.Data;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using System.IO;

namespace ProxyServer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult UploadFile()
        {
            return View();
        }

        //[Authorize]
        //[HttpPost]
        //public async Task<IActionResult> CreateSignatureStatement(SignatureStatement signatureStatement)
        //{
        //    signatureStatement.UserId = _context.Users.Where(user => user.UserName == User.Identity.Name).SingleOrDefault().Id;
        //    signatureStatement.Username = User.Identity.Name;
        //    signatureStatement.MessageDigest = "null";
        //    var size = signatureStatement.Files.Sum(file => file.Length);
        //    var filePaths = new List<string>();
        //    foreach (var formFile in signatureStatement.Files)
        //    {
        //        if (formFile.Length > 0)
        //        {
        //            // full path to file in temp location
        //            var filePath = Path.GetTempFileName(); //we are using Temp file name just for the example. Add your own file path.
        //            filePaths.Add(filePath);
        //            using (var stream = new FileStream(filePath, FileMode.Create))
        //            {
        //                await formFile.CopyToAsync(stream);
        //            }
        //        }
        //    }
        //    // process uploaded files
        //    // Don't rely on or trust the FileName property without validation.
        //    signatureStatement.SignedOn = DateTime.Now;
        //    return Ok(new { size, filePaths });
        //}

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateSignatureStatement(SignatureStatement signatureStatement)
        {
            signatureStatement.UserId = _context.Users.Where(user => user.UserName == User.Identity.Name).SingleOrDefault().Id;
            signatureStatement.Username = User.Identity.Name;
            using (var md5 = MD5.Create())
            {
                using var stream = signatureStatement.Files.OpenReadStream();
                var hash = md5.ComputeHash(stream);
                signatureStatement.MessageDigest = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
            signatureStatement.SignedOn = DateTime.Now;
            return View(signatureStatement);
        }

    }
}