
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Models;

namespace RaceDay.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EventCategoriesController : ControllerBase
    {
        private readonly RaceDayContext _context;

        public EventCategoriesController(RaceDayContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetEventCategories()
        {
            var eventCategories = await _context.EventCategories
                .AsNoTracking()
                .Select(ec => new
                {
                    ec.EventCategoryID,
                    ec.EventID,
                    EventName = ec.Event!.EventName,
                    ec.CategoryID,
                    CategoryName = ec.Category!.CategoryName,
                    ec.EntryFee,
                    ec.MaximumParticipants
                })
                .ToListAsync();

            return Ok(eventCategories);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetEventCategory(int id)
        {
            var eventCategory = await _context.EventCategories
                .AsNoTracking()
                .Where(ec => ec.EventCategoryID == id)
                .Select(ec => new
                {
                    ec.EventCategoryID,
                    ec.EventID,
                    EventName = ec.Event!.EventName,
                    ec.CategoryID,
                    CategoryName = ec.Category!.CategoryName,
                    ec.EntryFee,
                    ec.MaximumParticipants
                })
                .FirstOrDefaultAsync();

            if (eventCategory == null)
            {
                return NotFound(new
                {
                    message = "Event category not found."
                });
            }

            return Ok(eventCategory);
        }

        [Authorize(Roles = "Organiser")]
        [HttpPost]
        public async Task<IActionResult> CreateEventCategory(
            [FromBody] CreateEventCategoryRequest request)
        {
            if (request.EventID <= 0 || request.CategoryID <= 0)
            {
                return BadRequest(new
                {
                    message = "Valid EventID and CategoryID are required."
                });
            }

            if (request.EntryFee < 0 ||
                (request.MaximumParticipants.HasValue &&
                 request.MaximumParticipants.Value <= 0))
            {
                return BadRequest(new
                {
                    message = "Entry fee cannot be negative, and maximum participants must be greater than zero."
                });
            }

            var userIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out int userId))
            {
                return Unauthorized(new
                {
                    message = "User identity could not be verified."
                });
            }

            var eventItem = await _context.Events
                .FirstOrDefaultAsync(e => e.EventID == request.EventID);

            if (eventItem == null)
            {
                return NotFound(new
                {
                    message = "The specified event does not exist."
                });
            }

            if (eventItem.OrganiserID != userId)
            {
                return Forbid();
            }

            bool categoryExists = await _context.Categories
                .AnyAsync(c => c.CategoryID == request.CategoryID);

            if (!categoryExists)
            {
                return BadRequest(new
                {
                    message = "The specified category does not exist."
                });
            }

            bool alreadyLinked = await _context.EventCategories
                .AnyAsync(ec =>
                    ec.EventID == request.EventID &&
                    ec.CategoryID == request.CategoryID);

            if (alreadyLinked)
            {
                return Conflict(new
                {
                    message = "This category is already linked to the event."
                });
            }

            var eventCategory = new EventCategory
            {
                EventID = request.EventID,
                CategoryID = request.CategoryID,
                EntryFee = request.EntryFee,
                MaximumParticipants = request.MaximumParticipants
            };

            _context.EventCategories.Add(eventCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEventCategory),
                new { id = eventCategory.EventCategoryID },
                new
                {
                    eventCategory.EventCategoryID,
                    eventCategory.EventID,
                    eventCategory.CategoryID,
                    eventCategory.EntryFee,
                    eventCategory.MaximumParticipants
                });
        }
    }
}
