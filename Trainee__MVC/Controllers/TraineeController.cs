using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using Trainee__MVC.Models;

namespace TraineeProject_ClientSide.Controllers
{
    public class TraineeController : Controller
    {
        IConfiguration _configuration;
        string BaseURL;
        public TraineeController(ILogger<TraineeController> logger, IConfiguration configuration)
        {

            _configuration = configuration;
            BaseURL = _configuration.GetValue<string>("BaseURL");

        }
        public async Task<ActionResult> Index()
        {
            List<Trainee> trainees = await DisplayTrainees();
            return View(trainees);
        }
        public async Task<ActionResult> Details(int id)
        {
            Trainee trainee = await DisplayTrainees(id);
            return View(trainee);
        }
        // GET: TraineesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TraineesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Trainee trainee)
        {

            var AccessMail = HttpContext.Session.GetString("Email");
            Trainee receivedTrainee = new Trainee();

            HttpClientHandler clientHandler = new HttpClientHandler();


            var httpClient = new HttpClient(clientHandler);


            StringContent content = new StringContent(JsonConvert.SerializeObject(trainee), Encoding.UTF8, "application/json");

            using (var response = await httpClient.PostAsync(BaseURL + "api/Trainees", content))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                receivedTrainee = JsonConvert.DeserializeObject<Trainee>(apiResponse);
                if (receivedTrainee != null)
                {
                    return RedirectToAction("Login");
                }
            }


            ViewBag.Message = "Sorry Please try again!!!!!...";
            return View();


        }
        // GET: TraineesController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            Trainee trainee = await DisplayTrainees(id);
            return View(trainee);
        }


        // POST: TraineesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Trainee UpdatedTrainee)
        {
            UpdatedTrainee.TraineeID = id;
            var accessToken = HttpContext.Session.GetString("Email");


            HttpClientHandler clientHandler = new HttpClientHandler();

            var httpClient = new HttpClient(clientHandler);
            StringContent contents = new StringContent(JsonConvert.SerializeObject(UpdatedTrainee), Encoding.UTF8, "application/json");

            using (var response = await httpClient.PutAsync(BaseURL + "/api/Trainees/" + id, contents))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();

                if (apiResponse != null)
                    return RedirectToAction("Index");
                else
                    return View();
            }
        }

        // GET: TraineesController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            Trainee trainee = await DisplayTrainees(id);
            return View(trainee);
        }

        // POST: TraineesController/Delete/5
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                var accessEmail = HttpContext.Session.GetString("Email");
                HttpClientHandler clientHandler = new HttpClientHandler();
                var httpClient = new HttpClient(clientHandler);
                var response = await httpClient.DeleteAsync(BaseURL + "/api/Trainees/" + id);
                string apiResponse = await response.Content.ReadAsStringAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public async Task<List<Trainee>> DisplayTrainees()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            HttpClient client = new HttpClient(clientHandler);

            string JsonStr = await client.GetStringAsync(BaseURL + "/api/Trainees");
            var result = JsonConvert.DeserializeObject<List<Trainee>>(JsonStr);
            return result;
        }

        public async Task<Trainee> DisplayTrainees(int id)
        {

            var accessEmail = HttpContext.Session.GetString("Email");
            Trainee receivedTrainee = new Trainee();

            HttpClientHandler clientHandler = new HttpClientHandler();

            var httpClient = new HttpClient(clientHandler);

            using (var response = await httpClient.GetAsync(BaseURL + "/api/Trainees/" + id))
            {
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    receivedTrainee = JsonConvert.DeserializeObject<Trainee>(apiResponse);
                }
                else
                    ViewBag.StatusCode = response.StatusCode;
            }
            return receivedTrainee;
        }



        [HttpGet]
        public IActionResult ViewMapped()
        {
            return View();
        }
        public async Task<IActionResult> Filtered(Trainee mapping)
        {
            ViewBag.Mapping = mapping;
            if (mapping != null)
            {
                List<Trainee> trainees = await DisplayTrainees();
                var filterd = trainees.Where(a => a.MappedTo.Equals(mapping.MappedTo)).ToList();

                return View(filterd);
            }
            else
            {
                return RedirectToAction("ViewMapped");
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Trainee trainee)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Message = "Inputs are not valid";
                return View();
            }
            List<Trainee> trainees = await DisplayTrainees();
            var obj = trainees.Where(a => a.EmailId.Equals(trainee.EmailId) && a.Password.Equals(trainee.Password)).FirstOrDefault();
            if (obj != null)
            {
                HttpContext.Session.SetString("Email", obj.EmailId.ToString());
                return RedirectToAction("DashBoard", "Trainee");
            }
            else
            {
                ViewBag.Message = "User not found for given Email and Password";
                
                return View();
            }
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
