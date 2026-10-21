
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDay.DTOs;
using System.Security.Claims;

namespace RaceDay.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            string? loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string? role = User.FindFirstValue(ClaimTypes.Role);

            if (loggedInUserId != id.ToString() && role != "Organiser")
            {
                return Forbid();
            }

            return Ok(new
            {
                message = "Get user endpoint is working.",
                userId = id
            });
        }

        [HttpGet("{id}/profile")]
        public IActionResult GetProfile(int id)
        {
            string? loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string? role = User.FindFirstValue(ClaimTypes.Role);

            if (loggedInUserId != id.ToString() && role != "Organiser")
            {
                return Forbid();
            }

            return Ok(new
            {
                message = "Get profile endpoint is working.",
                userId = id
            });
        }

        [HttpPut("{id}/profile")]
        public IActionResult UpdateProfile(int id, UpdateProfileRequest request)
        {
            string? loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (loggedInUserId != id.ToString())
            {
                return Forbid();
            }

            return Ok(new
            {
                message = "Update profile endpoint is working.",
                userId = id,
                data = request
            });
        }
    }
}
