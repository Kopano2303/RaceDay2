
using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Services;
using RaceDay.Mvc.ViewModels;
using System.Net.Http.Json;
using System.Text.Json;

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
                ViewBag.ErrorMessage = "Unable to load categories.";
                return View(new List<CategoryViewModel>());
            }

            var categories = await response.Content
                .ReadFromJsonAsync<List<CategoryViewModel>>();

            return View(categories ?? new List<CategoryViewModel>());
        }

        public async Task<IActionResult> EventCategories()
        {
            string? token = HttpContext.Session.GetString("AuthToken");

            var response = await _apiService.SendAsync(
                HttpMethod.Get,
                "api/EventCategories",
                token: token);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = "Unable to load event categories.";
                return View(new List<EventCategoryViewModel>());
            }

            var eventCategories = await response.Content
                .ReadFromJsonAsync<List<EventCategoryViewModel>>();

            return View(eventCategories ?? new List<EventCategoryViewModel>());
        }

        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(CategoryViewModel model)
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
                TempData["SuccessMessage"] = "Category created successfully.";
                return RedirectToAction("Categories");
            }

            ViewBag.ErrorMessage = "Unable to create category.";
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> CreateEventCategory()
        {
            var model = new CreateEventCategoryViewModel();
            await LoadEventCategoryOptions(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEventCategory(
            CreateEventCategoryViewModel model)
        {
            if (model.EventID <= 0)
            {
                ModelState.AddModelError("EventID", "Please select an event.");
            }

            if (model.CategoryID <= 0)
            {
                ModelState.AddModelError("CategoryID", "Please select a category.");
            }

            if (!ModelState.IsValid)
            {
                await LoadEventCategoryOptions(model);
                return View(model);
            }

            string? token = HttpContext.Session.GetString("AuthToken");

            var response = await _apiService.SendAsync(
                HttpMethod.Post,
                "api/EventCategories",
                new
                {
                    model.EventID,
                    model.CategoryID,
                    model.EntryFee,
                    model.MaximumParticipants
                },
                token);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] =
                    "Category successfully linked to the event.";

                return RedirectToAction("EventCategories");
            }

            ViewBag.ErrorMessage = "Unable to link the category.";
            await LoadEventCategoryOptions(model);
            return View(model);
        }

        private async Task LoadEventCategoryOptions(
            CreateEventCategoryViewModel model)
        {
            string? token = HttpContext.Session.GetString("AuthToken");

            var eventResponse = await _apiService.SendAsync(
                HttpMethod.Get,
                "api/Events",
                token: token);

            if (eventResponse.IsSuccessStatusCode)
            {
                model.Events = await eventResponse.Content
                    .ReadFromJsonAsync<List<EventViewModel>>()
                    ?? new List<EventViewModel>();
            }

            var categoryResponse = await _apiService.SendAsync(
                HttpMethod.Get,
                "api/Categories",
                token: token);

            if (categoryResponse.IsSuccessStatusCode)
            {
                model.Categories = await categoryResponse.Content
                    .ReadFromJsonAsync<List<CategoryViewModel>>()
                    ?? new List<CategoryViewModel>();
            }
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
                ViewBag.ErrorMessage = "Unable to load events.";
                return View(new List<EventViewModel>());
            }

            var events = await response.Content
                .ReadFromJsonAsync<List<EventViewModel>>();

            return View(events ?? new List<EventViewModel>());
        }

        [HttpGet]
        public IActionResult CreateEvent()
        {
            return View(new EventViewModel
            {
                EventDate = DateTime.Today,
                StartTime = TimeSpan.FromHours(8),
                EventType = "Run",
                Status = "Upcoming"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEvent(EventViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string? token = HttpContext.Session.GetString("AuthToken");
            string? userId = HttpContext.Session.GetString("UserID");

            if (!int.TryParse(userId, out int organiserId))
            {
                ViewBag.ErrorMessage =
                    "Unable to identify the logged-in organiser.";

                return View(model);
            }

            var response = await _apiService.SendAsync(
                HttpMethod.Post,
                "api/Events",
                new
                {
                    OrganiserID = organiserId,
                    model.EventName,
                    model.Description,
                    model.EventType,
                    model.EventDate,
                    model.StartTime,
                    model.Venue,
                    model.City,
                    model.Province,
                    model.Status
                },
                token);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Event created successfully.";
                return RedirectToAction("Events");
            }

            ViewBag.ErrorMessage =
                "Unable to create the event. Please check the information.";

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditEvent(int id)
        {
            string? token = HttpContext.Session.GetString("AuthToken");

            var response = await _apiService.SendAsync(
                HttpMethod.Get,
                $"api/Events/{id}",
                token: token);

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Unable to find the event.";
                return RedirectToAction("Events");
            }

            var model = await response.Content
                .ReadFromJsonAsync<EventViewModel>();

            if (model == null)
            {
                TempData["ErrorMessage"] = "Unable to load event details.";
                return RedirectToAction("Events");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEvent(
            int id,
            EventViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string? token = HttpContext.Session.GetString("AuthToken");
            string? userId = HttpContext.Session.GetString("UserID");

            if (!int.TryParse(userId, out int organiserId))
            {
                ViewBag.ErrorMessage =
                    "Unable to identify the logged-in organiser.";

                return View(model);
            }

            var response = await _apiService.SendAsync(
                HttpMethod.Put,
                $"api/Events/{id}",
                new
                {
                    OrganiserID = organiserId,
                    model.EventName,
                    model.Description,
                    model.EventType,
                    model.EventDate,
                    model.StartTime,
                    model.Venue,
                    model.City,
                    model.Province,
                    model.Status
                },
                token);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Event updated successfully.";
                return RedirectToAction("Events");
            }

            string errorMessage = "Unable to update the event.";

            try
            {
                var error = await response.Content
                    .ReadFromJsonAsync<JsonElement>();

                if (error.TryGetProperty("message", out var message))
                {
                    errorMessage = message.GetString() ?? errorMessage;
                }
            }
            catch
            {
            }

            ViewBag.ErrorMessage = errorMessage;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            string? token = HttpContext.Session.GetString("AuthToken");

            var response = await _apiService.SendAsync(
                HttpMethod.Delete,
                $"api/Events/{id}",
                token: token);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Event deleted successfully.";
            }
            else
            {
                string errorMessage = "Unable to delete the event.";

                try
                {
                    var error = await response.Content
                        .ReadFromJsonAsync<JsonElement>();

                    if (error.TryGetProperty("message", out var message))
                    {
                        errorMessage = message.GetString() ?? errorMessage;
                    }
                }
                catch
                {
                }

                TempData["ErrorMessage"] = errorMessage;
            }

            return RedirectToAction("Events");
        }

        public async Task<IActionResult> Enrolments()
        {
            string? token = HttpContext.Session.GetString("AuthToken");

            var response = await _apiService.SendAsync(
                HttpMethod.Get,
                "api/Events",
                token: token);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = "Unable to load events.";
                return View(new List<EventViewModel>());
            }

            var events = await response.Content
                .ReadFromJsonAsync<List<EventViewModel>>();

            return View(events ?? new List<EventViewModel>());
        }

        public IActionResult Results()
        {
            return View();
        }
    }
}

