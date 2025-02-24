using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using Showcase_Contactpagina.Models;
using System.Numerics;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Showcase_Contactpagina.Controllers
{
    public class ContactController : Controller
    {
        private readonly HttpClient _httpClient;
        public ContactController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7278");
        }

        // GET: ContactController
        public ActionResult Index()
        {
            return View();
        }

        public async Task<bool> VerifyCaptcha(string captchaResponse)
        {
            string secretKey = "6LeuBtYqAAAAAFd63Jc2ojDJKM8LRk0NVV2o-Kr7";

            using (HttpClient client = new HttpClient())
            {
                var values = new Dictionary<string, string>
        {
            { "secret", secretKey },
            { "response", captchaResponse }
        };

                var content = new FormUrlEncodedContent(values);
                var response = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);
                var responseString = await response.Content.ReadAsStringAsync();

                // Parse the JSON response from Google reCAPTCHA
                var jsonResponse = Newtonsoft.Json.Linq.JObject.Parse(responseString);
                bool success = jsonResponse["success"].ToObject<bool>();

                return success;
            }
        }

        // POST: ContactController
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(Contactform form)
        {
            var captchaToken = Request.Form["g-recaptcha-response"];
            Console.WriteLine(captchaToken);

            bool captchaValid = await VerifyCaptcha(captchaToken);
            
            if(!ModelState.IsValid || !captchaValid)
            {
                ViewBag.Message = "De ingevulde velden voldoen niet aan de gestelde voorwaarden";
                return View();
            }

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var json = JsonConvert.SerializeObject(form, settings);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            //Gebruik _httpClient om een POST-request te doen naar ShowcaseAPI die de Mail uiteindelijk verstuurt met Mailtrap (of een alternatief).
            //Verstuur de gegevens van het ingevulde formulier mee aan de API, zodat dit per mail verstuurd kan worden naar de ontvanger.
            //Hint: je kunt dit met één regel code doen. Niet te moeilijk denken dus. :-)
            //Hint: vergeet niet om de mailfunctionaliteit werkend te maken in ShowcaseAPI > Controllers > MailController.cs,
            //      nadat je een account hebt aangemaakt op Mailtrap (of een alternatief).

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7278/api/Mail") { Content = content };
            HttpResponseMessage response = await _httpClient.SendAsync(request);


            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Message = "Er is iets misgegaan";
                return View();
            }

            ViewBag.Message = "Het contactformulier is verstuurd";
            
            // Clear the user's data
            form = null;

            ModelState.Clear();
            return View();
        }
    }
}
