
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using RaceDay.Data;
using RaceDay.Models;

namespace RaceDay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WeatherController : ControllerBase
    {
        private readonly RaceDayContext _context;

        public WeatherController(RaceDayContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetWeather()
        {
            var records = await _context.WeatherRecords
                .Select(w => new
                {
                    w.WeatherID,
                    w.EventID,
                    EventName = w.Event != null ? w.Event.EventName : null,
                    w.Temperature,
                    w.FeelsLike,
                    w.Humidity,
                    w.WindSpeed,
                    w.WeatherCondition,
                    w.RecordedAt
                })
                .ToListAsync();

            return Ok(records);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetWeatherRecord(int id)
        {
            var record = await _context.WeatherRecords
                .Where(w => w.WeatherID == id)
                .Select(w => new
                {
                    w.WeatherID,
                    w.EventID,
                    EventName = w.Event != null ? w.Event.EventName : null,
                    w.Temperature,
                    w.FeelsLike,
                    w.Humidity,
                    w.WindSpeed,
                    w.WeatherCondition,
                    w.RecordedAt
                })
                .FirstOrDefaultAsync();

            if (record == null)
            {
                return NotFound(new
                {
                    message = "Weather record not found."
                });
            }

            return Ok(record);
        }

        [HttpGet("event/{eventId:int}")]
        public async Task<IActionResult> GetEventWeather(int eventId)
        {
            var eventExists = await _context.Events
                .AnyAsync(e => e.EventID == eventId);

            if (!eventExists)
            {
                return NotFound(new
                {
                    message = "Event not found."
                });
            }

            var records = await _context.WeatherRecords
                .Where(w => w.EventID == eventId)
                .OrderByDescending(w => w.RecordedAt)
                .Select(w => new
                {
                    w.WeatherID,
                    w.EventID,
                    EventName = w.Event != null ? w.Event.EventName : null,
                    w.Temperature,
                    w.FeelsLike,
                    w.Humidity,
                    w.WindSpeed,
                    w.WeatherCondition,
                    w.RecordedAt
                })
                .ToListAsync();

            return Ok(records);
        }

        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> CreateWeatherRecord(
            [FromBody] Weather request)
        {
            if (request.EventID <= 0)
            {
                return BadRequest(new
                {
                    message = "A valid EventID is required."
                });
            }

            if (request.Humidity.HasValue &&
                (request.Humidity.Value < 0 || request.Humidity.Value > 100))
            {
                return BadRequest(new
                {
                    message = "Humidity must be between 0 and 100."
                });
            }

            if (request.WindSpeed.HasValue && request.WindSpeed.Value < 0)
            {
                return BadRequest(new
                {
                    message = "Wind speed cannot be negative."
                });
            }

            var organiserId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(organiserId, out int userId))
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
                    message = "Event not found."
                });
            }

            if (eventItem.OrganiserID != userId)
            {
                return Forbid();
            }

            var record = new Weather
            {
                EventID = request.EventID,
                Temperature = request.Temperature,
                FeelsLike = request.FeelsLike,
                Humidity = request.Humidity,
                WindSpeed = request.WindSpeed,
                WeatherCondition = request.WeatherCondition,
                RecordedAt = DateTime.UtcNow
            };

            _context.WeatherRecords.Add(record);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetWeatherRecord),
                new { id = record.WeatherID },
                record);
        }
    }
}
