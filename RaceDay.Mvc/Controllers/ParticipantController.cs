
using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Services;
using RaceDay.Mvc.ViewModels;
using System.Net.Http.Json;
using System.Text.Json;

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

            var eventResponse = await _apiService.SendAsync(
                HttpMethod.Get,
                "api/Events",
                token: token);

            if (!eventResponse.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage =
                    "Unable to load events. Please try again.";

                return View(new List<EventViewModel>());
            }

            var events = await eventResponse.Content
                .ReadFromJsonAsync<List<EventViewModel>>();

            if (events == null)
            {
                events = new List<EventViewModel>();
            }

            var categoryResponse = await _apiService.SendAsync(
                HttpMethod.Get,
                "api/EventCategories",
                token: token);

            if (categoryResponse.IsSuccessStatusCode)
            {
                var categories = await categoryResponse.Content
                    .ReadFromJsonAsync<List<EventCategoryViewModel>>();

                if (categories != null)
                {
                    foreach (var eventItem in events)
                    {
                        eventItem.Categories = categories
                            .Where(c => c.EventID == eventItem.EventID)
                            .ToList();
                    }
                }
            }
            else
            {
                ViewBag.CategoryErrorMessage =
                    "Event categories could not be loaded.";
            }

            return View(events);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enrol(int eventCategoryId)
        {
            string? token = HttpContext.Session.GetString("AuthToken");

            var response = await _apiService.SendAsync(
                HttpMethod.Post,
                "api/Enrolments",
                new
                {
                    EventCategoryID = eventCategoryId
                },
                token);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] =
                    "You have successfully enrolled in this event!";
            }
            else
            {
                string errorMessage =
                    "Unable to complete your enrolment.";

                try
                {
                    var error = await response.Content
                        .ReadFromJsonAsync<JsonElement>();

                    if (error.TryGetProperty("message", out var message))
                    {
                        errorMessage =
                            message.GetString() ?? errorMessage;
                    }
                }
                catch
                {
                }

                TempData["ErrorMessage"] = errorMessage;
            }

            return RedirectToAction("Events");
        }

        public async Task<IActionResult> MyEnrolments()
        {
            string? token =
                HttpContext.Session.GetString("AuthToken");

            var response = await _apiService.SendAsync(
                HttpMethod.Get,
                "api/Enrolments/my",
                token: token);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage =
                    "Unable to load your enrolments. Please try again.";

                return View(new List<EnrolmentViewModel>());
            }

            var enrolments = await response.Content
                .ReadFromJsonAsync<List<EnrolmentViewModel>>();

            return View(enrolments ?? new List<EnrolmentViewModel>());
        }

        public async Task<IActionResult> MyResults()
        {
            string? token =
                HttpContext.Session.GetString("AuthToken");

            var response = await _apiService.SendAsync(
                HttpMethod.Get,
                "api/Results/my",
                token: token);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage =
                    "Unable to load your race results. Please try again.";

                return View(new List<ResultViewModel>());
            }

            var results = await response.Content
                .ReadFromJsonAsync<List<ResultViewModel>>();

            return View(results ?? new List<ResultViewModel>());
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            string? token =
                HttpContext.Session.GetString("AuthToken");

            string? userId =
                HttpContext.Session.GetString("UserID");

            if (!int.TryParse(userId, out int id))
            {
                return RedirectToAction("Login", "Account");
            }

            var response = await _apiService.SendAsync(
                HttpMethod.Get,
                $"api/Users/{id}/profile",
                token: token);

            if (response.StatusCode ==
                System.Net.HttpStatusCode.NotFound)
            {
                return View(new ProfileViewModel
                {
                    UserID = id
                });
            }

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage =
                    "Unable to load your profile. Please try again.";

                return View(new ProfileViewModel
                {
                    UserID = id
                });
            }

            var profile = await response.Content
                .ReadFromJsonAsync<ProfileViewModel>();

            if (profile == null)
            {
                ViewBag.ErrorMessage =
                    "The API returned invalid profile information.";

                return View(new ProfileViewModel
                {
                    UserID = id
                });
            }

            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            string? token =
                HttpContext.Session.GetString("AuthToken");

            string? userId =
                HttpContext.Session.GetString("UserID");

            if (!int.TryParse(userId, out int id))
            {
                return RedirectToAction("Login", "Account");
            }

            model.UserID = id;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _apiService.SendAsync(
                HttpMethod.Put,
                $"api/Users/{id}/profile",
                new
                {
                    model.DateOfBirth,
                    model.Gender,
                    model.EmergencyContactName,
                    model.EmergencyContactPhone,
                    model.ProfileImageUrl
                },
                token);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] =
                    "Your profile has been saved successfully.";

                return RedirectToAction("Profile");
            }

            ViewBag.ErrorMessage =
                "Unable to save your profile. Please try again.";

            try
            {
                var error = await response.Content
                    .ReadFromJsonAsync<JsonElement>();

                if (error.TryGetProperty("message", out var message))
                {
                    ViewBag.ErrorMessage =
                        message.GetString() ??
                        ViewBag.ErrorMessage;
                }
            }
            catch
            {
            }

            return View(model);
        }
    }
}

