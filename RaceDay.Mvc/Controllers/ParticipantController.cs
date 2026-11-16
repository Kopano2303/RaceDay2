
using Microsoft.AspNetCore.Mvc;

namespace RaceDay.Mvc.Controllers
{
    public class ParticipantController : BaseController
    {
        protected override string RequiredRole => "Participant";

        public IActionResult Index()
        {
            ViewData["Title"] = "Participant Dashboard";

            return View();
        }

        public IActionResult Events()
        {
            return View();
        }

        public IActionResult MyEnrolments()
        {
            return View();
        }

        public IActionResult MyResults()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }
    }
}
