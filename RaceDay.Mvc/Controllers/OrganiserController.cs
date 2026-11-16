
using Microsoft.AspNetCore.Mvc;

namespace RaceDay.Mvc.Controllers
{
    public class OrganiserController : BaseController
    {
        protected override string RequiredRole => "Organiser";

        public IActionResult Index()
        {
            ViewData["Title"] = "Organiser Dashboard";

            return View();
        }

        public IActionResult Events()
        {
            return View();
        }

        public IActionResult Categories()
        {
            return View();
        }

        public IActionResult Enrolments()
        {
            return View();
        }

        public IActionResult Results()
        {
            return View();
        }
    }
}
