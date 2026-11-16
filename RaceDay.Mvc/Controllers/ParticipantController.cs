
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using RaceDay.Mvc.Controllers;
using RaceDay.Mvc.Services;
using RaceDay.Mvc.ViewModels;

namespace RaceDay.Mvc.Controllers
{
    public class ParticipantController : BaseController
    {
        private readonly RaceDayApiService _apiService;

        public ParticipantController(RaceDayApiService apiService)
        {
            _apiService = apiService;
        }

        protected override string RequiredRole => "Participant";

        public IActionResult Index()
        {
            ViewData["Title"] = "Participant Dashboard";
            return View();
        }

        public async Task<IActionResult> Events()
        {
            string? token = HttpContext.Session.GetString("AuthToken");

            var response = await _apiService.SendAsync(
                HttpMethod.Get,
                "api/Events",
                token: token);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage =
                    "Unable to load events. Please try again.";

                return View(new List<EventViewModel>());
            }

            var events = await response.Content
                .ReadFromJsonAsync<List<EventViewModel>>();

            return View(events ?? new List<EventViewModel>());
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

