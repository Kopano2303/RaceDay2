
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
    public class EventsController : ControllerBase
    {
        private readonly RaceDayContext _context;

        public EventsController(RaceDayContext context)
        {
            _context = context;
        }

        private int GetLoggedInUserId()
        {
            string? userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            return int.TryParse(userId, out int id) ? id : 0;
        }

        private static bool IsValidEventType(string eventType)
        {
            string[] allowedTypes = { "Run", "Walk", "Cycle" };

            return allowedTypes.Contains(
                eventType.Trim(),
                StringComparer.OrdinalIgnoreCase);
        }

        private static bool HasValidEventDetails(CreateEventRequest request)
        {
            return !string.IsNullOrWhiteSpace(request.EventName)
                && !string.IsNullOrWhiteSpace(request.EventType)
                && IsValidEventType(request.EventType)
                && !string.IsNullOrWhiteSpace(request.Venue)
                && !string.IsNullOrWhiteSpace(request.City)
                && !string.IsNullOrWhiteSpace(request.Province)
                && !string.IsNullOrWhiteSpace(request.Status)
                && request.EventDate != default
                && request.StartTime >= TimeSpan.Zero
                && request.StartTime < TimeSpan.FromDays(1);
        }

        private static object EventResponse(RaceDay.Models.Event e)
        {
            return new
            {
                e.EventID,
                e.OrganiserID,
                e.EventName,
                e.Description,
                e.EventType,
                e.EventDate,
                e.StartTime,
                e.Venue,
                e.City,
                e.Province,
                e.Status
            };
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _context.Events
                .AsNoTracking()
                .OrderBy(e => e.EventDate)
                .Select(e => new
                {
                    e.EventID,
                    e.OrganiserID,
                    e.EventName,
                    e.Description,
                    e.EventType,
                    e.EventDate,
                    e.StartTime,
                    e.Venue,
                    e.City,
                    e.Province,
                    e.Status
                })
                .ToListAsync();

            return Ok(events);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetEvent(int id)
        {
            var eventItem = await _context.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EventID == id);

            if (eventItem == null)
            {
                return NotFound(new
                {
                    message = "Event not found."
                });
            }

            return Ok(EventResponse(eventItem));
        }

        [Authorize(Roles = "Organiser")]
        [HttpPost]
        public async Task<IActionResult> CreateEvent(
            [FromBody] CreateEventRequest request)
        {
            int organiserId = GetLoggedInUserId();

            if (organiserId <= 0)
            {
                return Unauthorized(new
                {
                    message = "User identity could not be verified."
                });
            }

            if (!HasValidEventDetails(request))
            {
                return BadRequest(new
                {
                    message = "Provide a valid event name, event type (Run, Walk, or Cycle), venue, city, province, status, date, and start time."
                });
            }

            var eventItem = new RaceDay.Models.Event
            {
                OrganiserID = organiserId,
                EventName = request.EventName.Trim(),
                Description = string.IsNullOrWhiteSpace(request.Description)
                    ? null
                    : request.Description.Trim(),
                EventType = request.EventType.Trim(),
                EventDate = request.EventDate,
                StartTime = request.StartTime,
                Venue = request.Venue.Trim(),
                City = request.City.Trim(),
                Province = request.Province.Trim(),
                Status = request.Status.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _context.Events.Add(eventItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEvent),
                new { id = eventItem.EventID },
                new
                {
                    message = "Event created successfully.",
                    data = EventResponse(eventItem)
                });
        }

        [Authorize(Roles = "Organiser")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateEvent(
            int id,
            [FromBody] CreateEventRequest request)
        {
            int organiserId = GetLoggedInUserId();

            if (organiserId <= 0)
            {
                return Unauthorized(new
                {
                    message = "User identity could not be verified."
                });
            }

            var eventItem = await _context.Events
                .FirstOrDefaultAsync(e => e.EventID == id);

            if (eventItem == null)
            {
                return NotFound(new
                {
                    message = "Event not found."
                });
            }

            if (eventItem.OrganiserID != organiserId)
            {
                return Forbid();
            }

            if (!HasValidEventDetails(request))
            {
                return BadRequest(new
                {
                    message = "Provide a valid event name, event type (Run, Walk, or Cycle), venue, city, province, status, date, and start time."
                });
            }

            eventItem.EventName = request.EventName.Trim();
            eventItem.Description = string.IsNullOrWhiteSpace(request.Description)
                ? null
                : request.Description.Trim();
            eventItem.EventType = request.EventType.Trim();
            eventItem.EventDate = request.EventDate;
            eventItem.StartTime = request.StartTime;
            eventItem.Venue = request.Venue.Trim();
            eventItem.City = request.City.Trim();
            eventItem.Province = request.Province.Trim();
            eventItem.Status = request.Status.Trim();

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Event updated successfully.",
                data = EventResponse(eventItem)
            });
        }

        [Authorize(Roles = "Organiser")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            int organiserId = GetLoggedInUserId();

            if (organiserId <= 0)
            {
                return Unauthorized(new
                {
                    message = "User identity could not be verified."
                });
            }

            var eventItem = await _context.Events
                .FirstOrDefaultAsync(e => e.EventID == id);

            if (eventItem == null)
            {
                return NotFound(new
                {
                    message = "Event not found."
                });
            }

            if (eventItem.OrganiserID != organiserId)
            {
                return Forbid();
            }

            bool hasEventCategories = await _context.EventCategories
                .AnyAsync(ec => ec.EventID == id);

            bool hasRoute = await _context.Routes
                .AnyAsync(r => r.EventID == id);

            bool hasWeatherRecords = await _context.WeatherRecords
                .AnyAsync(w => w.EventID == id);

            if (hasEventCategories || hasRoute || hasWeatherRecords)
            {
                return Conflict(new
                {
                    message = "This event has linked categories, route, or weather records. Preserve these records and update the event status instead of deleting it."
                });
            }

            _context.Events.Remove(eventItem);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Event deleted successfully."
            });
        }
    }
}
