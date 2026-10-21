
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
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayContext _context;

        public ResultsController(RaceDayContext context)
        {
            _context = context;
        }

        private int GetLoggedInUserId()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userId, out int id) ? id : 0;
        }

        [Authorize(Roles = "Organiser")]
        [HttpPost]
        public async Task<IActionResult> CreateResult(CreateResultRequest request)
        {
            int organiserId = GetLoggedInUserId();

            if (organiserId == 0)
                return Unauthorized();

            if (string.IsNullOrWhiteSpace(request.Status))
                return BadRequest(new { message = "Result status is required." });

            if (request.OverallPosition.HasValue &&
                request.OverallPosition.Value < 1)
                return BadRequest(new { message = "Overall position must be at least 1." });

            if (request.CategoryPosition.HasValue &&
                request.CategoryPosition.Value < 1)
                return BadRequest(new { message = "Category position must be at least 1." });

            var enrolment = await _context.Enrolments
                .Include(e => e.EventCategory)
                    .ThenInclude(ec => ec!.Event)
                .FirstOrDefaultAsync(e => e.EnrolmentID == request.EnrolmentID);

            if (enrolment == null)
                return NotFound(new { message = "Enrolment not found." });

            if (enrolment.EventCategory?.Event == null ||
                enrolment.EventCategory.Event.OrganiserID != organiserId)
                return Forbid();

            bool resultExists = await _context.Results
                .AnyAsync(r => r.EnrolmentID == request.EnrolmentID);

            if (resultExists)
                return Conflict(new { message = "A result already exists for this enrolment." });

            var result = new Result
            {
                EnrolmentID = request.EnrolmentID,
                FinishTime = request.FinishTime,
                OverallPosition = request.OverallPosition,
                CategoryPosition = request.CategoryPosition,
                Status = request.Status.Trim(),
                RecordedAt = DateTime.UtcNow
            };

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Race result recorded successfully.",
                result.ResultID,
                result.EnrolmentID,
                result.FinishTime,
                result.OverallPosition,
                result.CategoryPosition,
                result.Status,
                result.RecordedAt
            });
        }

        [Authorize(Roles = "Participant")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyResults()
        {
            int userId = GetLoggedInUserId();

            if (userId == 0)
                return Unauthorized();

            var results = await _context.Results
                .AsNoTracking()
                .Where(r => r.Enrolment!.UserID == userId)
                .Select(r => new
                {
                    r.ResultID,
                    r.EnrolmentID,
                    EventName = r.Enrolment!.EventCategory!.Event!.EventName,
                    CategoryName = r.Enrolment.EventCategory.Category!.CategoryName,
                    r.FinishTime,
                    r.OverallPosition,
                    r.CategoryPosition,
                    r.Status,
                    r.RecordedAt
                })
                .ToListAsync();

            return Ok(results);
        }

        [Authorize(Roles = "Organiser")]
        [HttpGet("event/{eventId}")]
        public async Task<IActionResult> GetEventResults(int eventId)
        {
            int organiserId = GetLoggedInUserId();

            bool ownsEvent = await _context.Events.AnyAsync(e =>
                e.EventID == eventId &&
                e.OrganiserID == organiserId);

            if (!ownsEvent)
                return NotFound(new { message = "Event not found for this organiser." });

            var results = await _context.Results
                .AsNoTracking()
                .Where(r => r.Enrolment!.EventCategory!.EventID == eventId)
                .Select(r => new
                {
                    r.ResultID,
                    r.EnrolmentID,
                    ParticipantID = r.Enrolment!.UserID,
                    ParticipantName = r.Enrolment.User!.FirstName + " " +
                                      r.Enrolment.User.LastName,
                    CategoryName = r.Enrolment.EventCategory!.Category!.CategoryName,
                    r.FinishTime,
                    r.OverallPosition,
                    r.CategoryPosition,
                    r.Status,
                    r.RecordedAt
                })
                .ToListAsync();

            return Ok(results);
        }
    }
}
