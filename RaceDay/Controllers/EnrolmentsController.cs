
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
    public class EnrolmentsController : ControllerBase
    {
        private readonly RaceDayContext _context;

        public EnrolmentsController(RaceDayContext context)
        {
            _context = context;
        }

        private int GetLoggedInUserId()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userId, out int id) ? id : 0;
        }

        [Authorize(Roles = "Participant")]
        [HttpPost]
        public async Task<IActionResult> CreateEnrolment(
            CreateEnrolmentRequest request)
        {
            int userId = GetLoggedInUserId();

            if (userId == 0)
                return Unauthorized();

            var eventCategory = await _context.EventCategories
                .Include(ec => ec.Event)
                .FirstOrDefaultAsync(ec =>
                    ec.EventCategoryID == request.EventCategoryID);

            if (eventCategory == null)
                return BadRequest(new { message = "Event category not found." });

            if (eventCategory.Event == null ||
                eventCategory.Event.EventDate < DateTime.Today)
                return BadRequest(new { message = "This event is no longer available for enrolment." });

            if (eventCategory.MaximumParticipants.HasValue)
            {
                int currentCount = await _context.Enrolments.CountAsync(e =>
                    e.EventCategoryID == request.EventCategoryID &&
                    e.EnrolmentStatus != "Cancelled");

                if (currentCount >= eventCategory.MaximumParticipants.Value)
                    return Conflict(new { message = "This event category is full." });
            }

            bool alreadyEnrolled = await _context.Enrolments.AnyAsync(e =>
                e.UserID == userId &&
                e.EventCategoryID == request.EventCategoryID &&
                e.EnrolmentStatus != "Cancelled");

            if (alreadyEnrolled)
                return Conflict(new { message = "You are already enrolled in this category." });

            var enrolment = new Enrolment
            {
                UserID = userId,
                EventCategoryID = request.EventCategoryID,
                PaymentStatus = "Pending",
                EnrolmentStatus = "Confirmed"
            };

            _context.Enrolments.Add(enrolment);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Enrolment created successfully.",
                enrolment.EnrolmentID,
                enrolment.UserID,
                enrolment.EventCategoryID,
                enrolment.PaymentStatus,
                enrolment.EnrolmentStatus
            });
        }

        [Authorize(Roles = "Participant")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyEnrolments()
        {
            int userId = GetLoggedInUserId();

            var enrolments = await _context.Enrolments
                .AsNoTracking()
                .Where(e => e.UserID == userId)
                .Select(e => new
                {
                    e.EnrolmentID,
                    e.EventCategoryID,
                    EventName = e.EventCategory!.Event!.EventName,
                    CategoryName = e.EventCategory.Category!.CategoryName,
                    e.PaymentStatus,
                    e.EnrolmentStatus
                })
                .ToListAsync();

            return Ok(enrolments);
        }

        [Authorize(Roles = "Organiser")]
        [HttpGet("event/{eventId}")]
        public async Task<IActionResult> GetEventEnrolments(int eventId)
        {
            int organiserId = GetLoggedInUserId();

            bool ownsEvent = await _context.Events.AnyAsync(e =>
                e.EventID == eventId &&
                e.OrganiserID == organiserId);

            if (!ownsEvent)
                return NotFound(new { message = "Event not found for this organiser." });

            var enrolments = await _context.Enrolments
                .AsNoTracking()
                .Where(e => e.EventCategory!.EventID == eventId)
                .Select(e => new
                {
                    e.EnrolmentID,
                    e.UserID,
                    ParticipantName = e.User!.FirstName + " " + e.User.LastName,
                    e.EventCategoryID,
                    CategoryName = e.EventCategory.Category!.CategoryName,
                    e.PaymentStatus,
                    e.EnrolmentStatus
                })
                .ToListAsync();

            return Ok(enrolments);
        }
    }
}
