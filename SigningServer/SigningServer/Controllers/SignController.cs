using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using System.IO.Pipelines;
using ProxyServer.Data;
using System.Text;


namespace ProxyServer.Controllers
{
    public class SignController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly RSACryptoServiceProvider RSA;

        public SignController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _context = context;
            _logger = logger;
            RSA = new RSACryptoServiceProvider();
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("/Sign/RequestSignature")]
        public string RequestSignature()
        {
            StreamReader bodyStream = new StreamReader(HttpContext.Request.Body);
            Task<string> bodyText = bodyStream.ReadToEndAsync();
            bodyText.Wait();

            string digest = bodyText.Result;
            string id = _context.Users.Where(user => user.UserName == User.Identity.Name).SingleOrDefault().Id;

            UnicodeEncoding byte_converter = new UnicodeEncoding();
            byte[] digest_bytes = byte_converter.GetBytes(digest);
            byte[] signed_digest = RSA.SignData(digest_bytes, SHA256.Create());

            byte[] ID_bytes = byte_converter.GetBytes(id);
            byte[] final_signature = new byte[signed_digest.Length + ID_bytes.Length];
            Buffer.BlockCopy(signed_digest, 0, final_signature, 0, signed_digest.Length);
            Buffer.BlockCopy(ID_bytes, 0, final_signature, signed_digest.Length, ID_bytes.Length);

            return Convert.ToBase64String(final_signature);
        }

        /*
        // [Authorize]
        [HttpPost]
        public string RequestSignature(string hash) {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7096/api/Sign");

                //HTTP POST
                var postTask = client.PostAsJsonAsync<string>("hashed", hash);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return "Bravo";
                }
            }


            return "hahaahha";
        }
        */
    }
}
