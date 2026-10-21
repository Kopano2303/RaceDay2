
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Models;
using System.Security.Claims;

namespace RaceDay.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly RaceDayContext _context;

        public UsersController(RaceDayContext context)
        {
            _context = context;
        }

        private bool CanAccessUser(int id)
        {
            string? loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string? role = User.FindFirstValue(ClaimTypes.Role);

            return loggedInUserId == id.ToString() ||
                   role == "Organiser";
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            if (!CanAccessUser(id))
            {
                return Forbid();
            }

            var user = await _context.Users
                .Where(u => u.UserID == id)
                .Select(u => new
                {
                    u.UserID,
                    u.Email,
                    u.FirstName,
                    u.LastName,
                    u.PhoneNumber,
                    u.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(user);
        }

        [HttpGet("{id}/profile")]
        public async Task<IActionResult> GetProfile(int id)
        {
            if (!CanAccessUser(id))
            {
                return Forbid();
            }

            var profile = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.UserID == id);

            if (profile == null)
            {
                return NotFound(new
                {
                    message = "Profile not found. Update the profile to create one."
                });
            }

            return Ok(new
            {
                profile.ProfileID,
                profile.UserID,
                profile.DateOfBirth,
                profile.Gender,
                profile.EmergencyContactName,
                profile.EmergencyContactPhone,
                profile.ProfileImageUrl
            });
        }

        [HttpPut("{id}/profile")]
        public async Task<IActionResult> UpdateProfile(
            int id,
            UpdateProfileRequest request)
        {
            string? loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (loggedInUserId != id.ToString())
            {
                return Forbid();
            }

            bool userExists = await _context.Users
                .AnyAsync(u => u.UserID == id);

            if (!userExists)
            {
                return NotFound(new { message = "User not found." });
            }

            UserProfile? profile = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.UserID == id);

            if (profile == null)
            {
                profile = new UserProfile
                {
                    UserID = id
                };

                _context.UserProfiles.Add(profile);
            }

            profile.DateOfBirth = request.DateOfBirth;
            profile.Gender = request.Gender;
            profile.EmergencyContactName = request.EmergencyContactName;
            profile.EmergencyContactPhone = request.EmergencyContactPhone;
            profile.ProfileImageUrl = request.ProfileImageUrl;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Profile saved successfully.",
                profile.ProfileID,
                profile.UserID,
                profile.DateOfBirth,
                profile.Gender,
                profile.EmergencyContactName,
                profile.EmergencyContactPhone,
                profile.ProfileImageUrl
            });
        }
    }
}

