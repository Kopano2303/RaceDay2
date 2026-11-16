
using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Services;
using RaceDay.Mvc.ViewModels;
using System.Net.Http.Json;

namespace RaceDay.Mvc.Controllers
{
    public class OrganiserController : BaseController
    {
        private readonly RaceDayApiService _apiService;

        public OrganiserController(RaceDayApiService apiService)
        {
            _apiService = apiService;
        }

        protected override string RequiredRole => "Organiser";

        public IActionResult Index()
        {
            ViewData["Title"] = "Organiser Dashboard";
            return View();
        }

        public async Task<IActionResult> Categories()
        {
            string? token = HttpContext.Session.GetString("AuthToken");

            var response = await _apiService.SendAsync(
                HttpMethod.Get,
                "api/Categories",
                token: token);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage =
                    "Unable to load categories. Please try again.";

                return View(new List<CategoryViewModel>());
            }

            var categories = await response.Content
                .ReadFromJsonAsync<List<CategoryViewModel>>();

            return View(categories ?? new List<CategoryViewModel>());
        }

        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(
            CategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string? token = HttpContext.Session.GetString("AuthToken");

            var response = await _apiService.SendAsync(
                HttpMethod.Post,
                "api/Categories",
                new
                {
                    model.CategoryName,
                    model.DistanceKM,
                    model.CategoryType,
                    model.Description
                },
                token);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] =
                    "Category created successfully.";

                return RedirectToAction("Categories");
            }

            ViewBag.ErrorMessage =
                "Unable to create category. Please check the information.";

            return View(model);
        }

        public IActionResult Events()
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
