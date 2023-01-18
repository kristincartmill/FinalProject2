using FinalProject.UI.MVC.Models;//added for access to our custom Models (MessageViewModel)
using System.Net;//added for NetworkCredential
using System.Net.Mail;//added for email
using System.Web.Mvc;

namespace FinalProject.UI.MVC.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        //[Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }
    }
}
