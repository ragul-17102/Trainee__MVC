using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Text;
using Trainee__MVC.Models;


namespace TraineeProject_ClientSide.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        IConfiguration _configuration;
        string BaseURL;
        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            BaseURL = _configuration.GetValue<string>("BaseURL");

        }

        public IActionResult Index()
        {
            HttpContext.Session.Clear();
            return View();
        }

        

        public ActionResult Create()
        {
            return View();
        }

        // POST: TraineesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Admin admin)
        {

            var AccessMail = HttpContext.Session.GetString("Email");
            Admin receivedTrainee = new Admin();

            HttpClientHandler clientHandler = new HttpClientHandler();


            var httpClient = new HttpClient(clientHandler);


            StringContent content = new StringContent(JsonConvert.SerializeObject(admin), Encoding.UTF8, "application/json");

            using (var response = await httpClient.PostAsync(BaseURL + "/api/Admins", content))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                receivedTrainee = JsonConvert.DeserializeObject<Admin>(apiResponse);
                if (receivedTrainee != null)
                {
                    return RedirectToAction("Login");
                }
            }


            ViewBag.Message = "Sorry Please try again!!!!!...";
            return View();


        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Admin admin)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Message = "not valid..Please Try Agsin";
                return View();
            }
            List<Admin> admins = await GetAdmin();
            var obj = admins.Where(a => a.Email.Equals(admin.Email) && a.Password.Equals(admin.Password)).FirstOrDefault();
            if (obj != null)
            {
                HttpContext.Session.SetString("Email", obj.Email.ToString());
                return RedirectToAction("DashBoard", "Home");
            }
            else
            {
                ViewBag.Message = "User not found for given Email and Password";
                ViewBag.User = obj.Email;
                return View();
            }
        }





        [HttpGet]
        public async Task<List<Admin>> GetAdmin()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            HttpClient client = new HttpClient(clientHandler);

            string JsonStr = await client.GetStringAsync(BaseURL + "/api/Admins");
            var result = JsonConvert.DeserializeObject<List<Admin>>(JsonStr);
            return result;
        }

        public IActionResult DashBoard()
        {
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }

}