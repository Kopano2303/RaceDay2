
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
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(userId, out int id) ? id : 0;
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvent(int id)
        {
            var eventItem = await _context.Events
                .AsNoTracking()
                .Where(e => e.EventID == id)
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
                .FirstOrDefaultAsync();

            if (eventItem == null)
            {
                return NotFound(new { message = "Event not found." });
            }

            return Ok(eventItem);
        }

        [Authorize(Roles = "Organiser")]
        [HttpPost]
        public async Task<IActionResult> CreateEvent(CreateEventRequest request)
        {
            int organiserId = GetLoggedInUserId();

            if (organiserId == 0)
            {
                return Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(request.EventName) ||
                string.IsNullOrWhiteSpace(request.EventType) ||
                string.IsNullOrWhiteSpace(request.Venue) ||
                string.IsNullOrWhiteSpace(request.City) ||
                string.IsNullOrWhiteSpace(request.Province) ||
                string.IsNullOrWhiteSpace(request.Status))
            {
                return BadRequest(new
                {
                    message = "Event name, type, venue, city, province and status are required."
                });
            }

            var eventItem = new RaceDay.Models.Event
            {
                OrganiserID = organiserId,
                EventName = request.EventName.Trim(),
                Description = request.Description,
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
                    eventItem.EventID,
                    eventItem.OrganiserID,
                    eventItem.EventName,
                    eventItem.Description,
                    eventItem.EventType,
                    eventItem.EventDate,
                    eventItem.StartTime,
                    eventItem.Venue,
                    eventItem.City,
                    eventItem.Province,
                    eventItem.Status
                });
        }

        [Authorize(Roles = "Organiser")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(
            int id,
            CreateEventRequest request)
        {
            int organiserId = GetLoggedInUserId();

            var eventItem = await _context.Events
                .FirstOrDefaultAsync(e => e.EventID == id);

            if (eventItem == null)
            {
                return NotFound(new { message = "Event not found." });
            }

            if (eventItem.OrganiserID != organiserId)
            {
                return Forbid();
            }

            if (string.IsNullOrWhiteSpace(request.EventName) ||
                string.IsNullOrWhiteSpace(request.EventType) ||
                string.IsNullOrWhiteSpace(request.Venue) ||
                string.IsNullOrWhiteSpace(request.City) ||
                string.IsNullOrWhiteSpace(request.Province) ||
                string.IsNullOrWhiteSpace(request.Status))
            {
                return BadRequest(new
                {
                    message = "Event name, type, venue, city, province and status are required."
                });
            }

            eventItem.EventName = request.EventName.Trim();
            eventItem.Description = request.Description;
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
                eventItem.EventID,
                eventItem.OrganiserID,
                eventItem.EventName,
                eventItem.Description,
                eventItem.EventType,
                eventItem.EventDate,
                eventItem.StartTime,
                eventItem.Venue,
                eventItem.City,
                eventItem.Province,
                eventItem.Status
            });
        }

        [Authorize(Roles = "Organiser")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            int organiserId = GetLoggedInUserId();

            var eventItem = await _context.Events
                .FirstOrDefaultAsync(e => e.EventID == id);

            if (eventItem == null)
            {
                return NotFound(new { message = "Event not found." });
            }

            if (eventItem.OrganiserID != organiserId)
            {
                return Forbid();
            }

            _context.Events.Remove(eventItem);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Event deleted successfully." });
        }
    }
}
