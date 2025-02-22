using Microsoft.AspNetCore.Mvc;
using ShowcaseAPI.Models;
using System.Net.Mail;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ShowcaseAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        // POST api/<MailController>
        [HttpPost]
        public ActionResult Post([Bind("FirstName, LastName, Email, Phone")] Contactform form)
        {
            var client = new SmtpClient("sandbox.smtp.mailtrap.io", 2525)
            {
                Credentials = new NetworkCredential("a445e57117a33d", "ebd2b23f428d1a"),
                EnableSsl = true
            };

            string emailBody = $"Naam: {form.FirstName} {form.LastName}\n" +
                   $"Telefoon: {form.Phone}\n\n" +
                   $"\n{form.Message}";

            client.Send("noreply@showcasecontact.com", form.Email, form.Subject, emailBody);
            System.Console.WriteLine("Sent");

            return Ok();
        }
    }
}
