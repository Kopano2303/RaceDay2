
using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Services;
using RaceDay.Mvc.ViewModels;
using System.Net.Http.Json;

namespace RaceDay.Mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly RaceDayApiService _apiService;

        public AccountController(RaceDayApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _apiService.SendAsync(
                HttpMethod.Post,
                "api/Auth/login",
                new
                {
                    model.Email,
                    model.Password
                });

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content
                    .ReadFromJsonAsync<ApiError>();

                ModelState.AddModelError(
                    string.Empty,
                    error?.Message ?? "Login failed. Please check your details.");

                return View(model);
            }

            var result = await response.Content
                .ReadFromJsonAsync<LoginResponse>();

            if (result == null || string.IsNullOrWhiteSpace(result.Token))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The API returned an invalid login response.");

                return View(model);
            }

            HttpContext.Session.SetString("AuthToken", result.Token);
            HttpContext.Session.SetString("UserID", result.UserID.ToString());
            HttpContext.Session.SetString("UserName",
                result.FirstName + " " + result.LastName);
            HttpContext.Session.SetString("UserEmail", result.Email);
            HttpContext.Session.SetString("UserRole", result.Role);

            if (result.Role == "Organiser")
            {
                return RedirectToAction("Index", "Organiser");
            }

            return RedirectToAction("Index", "Participant");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _apiService.SendAsync(
                HttpMethod.Post,
                "api/Auth/register",
                new
                {
                    model.Email,
                    model.Password,
                    model.FirstName,
                    model.LastName,
                    model.PhoneNumber
                });

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content
                    .ReadFromJsonAsync<ApiError>();

                ModelState.AddModelError(
                    string.Empty,
                    error?.Message ?? "Registration failed.");

                return View(model);
            }

            TempData["SuccessMessage"] =
                "Your account has been created. Please log in.";

            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }

        private class ApiError
        {
            public string? Message { get; set; }
        }

        private class LoginResponse
        {
            public string Token { get; set; } = string.Empty;
            public int UserID { get; set; }
            public string Email { get; set; } = string.Empty;
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
        }
    }
}
