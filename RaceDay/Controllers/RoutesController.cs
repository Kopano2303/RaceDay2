
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Models;

namespace RaceDay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RoutesController : ControllerBase
    {
        private readonly RaceDayContext _context;

        public RoutesController(RaceDayContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetRoutes()
        {
            var routes = await _context.Routes
                .Select(r => new
                {
                    r.RouteID,
                    r.EventID,
                    EventName = r.Event != null ? r.Event.EventName : null,
                    r.RouteName,
                    r.DistanceKM,
                    r.ElevationGain,
                    r.RouteMapUrl,
                    r.Description
                })
                .ToListAsync();

            return Ok(routes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoute(int id)
        {
            var route = await _context.Routes
                .Where(r => r.RouteID == id)
                .Select(r => new
                {
                    r.RouteID,
                    r.EventID,
                    EventName = r.Event != null ? r.Event.EventName : null,
                    r.RouteName,
                    r.DistanceKM,
                    r.ElevationGain,
                    r.RouteMapUrl,
                    r.Description
                })
                .FirstOrDefaultAsync();

            if (route == null)
            {
                return NotFound(new { message = "Route not found." });
            }

            return Ok(route);
        }

        [HttpGet("event/{eventId}")]
        public async Task<IActionResult> GetRouteByEvent(int eventId)
        {
            var eventExists = await _context.Events
                .AnyAsync(e => e.EventID == eventId);

            if (!eventExists)
            {
                return NotFound(new { message = "Event not found." });
            }

            var routes = await _context.Routes
                .Where(r => r.EventID == eventId)
                .Select(r => new
                {
                    r.RouteID,
                    r.EventID,
                    EventName = r.Event != null ? r.Event.EventName : null,
                    r.RouteName,
                    r.DistanceKM,
                    r.ElevationGain,
                    r.RouteMapUrl,
                    r.Description
                })
                .ToListAsync();

            return Ok(routes);
        }

        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> CreateRoute(CreateRouteRequest request)
        {
            if (request.EventID <= 0 ||
                string.IsNullOrWhiteSpace(request.RouteName) ||
                request.DistanceKM <= 0 ||
                (request.ElevationGain.HasValue && request.ElevationGain.Value < 0))
            {
                return BadRequest(new
                {
                    message = "Provide a valid event, route name, positive distance and non-negative elevation."
                });
            }

            if (!string.IsNullOrWhiteSpace(request.RouteMapUrl) &&
                (!Uri.TryCreate(request.RouteMapUrl, UriKind.Absolute, out var mapUri) ||
                 (mapUri.Scheme != Uri.UriSchemeHttp &&
                  mapUri.Scheme != Uri.UriSchemeHttps)))
            {
                return BadRequest(new
                {
                    message = "RouteMapUrl must be a valid HTTP or HTTPS URL."
                });
            }

            var organiserId = GetUserId();

            if (organiserId == null)
            {
                return Unauthorized(new { message = "User identity could not be verified." });
            }

            var eventItem = await _context.Events
                .FirstOrDefaultAsync(e => e.EventID == request.EventID);

            if (eventItem == null)
            {
                return NotFound(new { message = "Event not found." });
            }

            if (eventItem.OrganiserID != organiserId.Value)
            {
                return Forbid();
            }

            var route = new RaceDay.Models.Route
            {
                EventID = request.EventID,
                RouteName = request.RouteName.Trim(),
                DistanceKM = request.DistanceKM,
                ElevationGain = request.ElevationGain,
                RouteMapUrl = request.RouteMapUrl,
                Description = request.Description
            };

            _context.Routes.Add(route);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetRoute),
                new { id = route.RouteID },
                route);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateRoute(int id, CreateRouteRequest request)
        {
            if (request.EventID <= 0 ||
                string.IsNullOrWhiteSpace(request.RouteName) ||
                request.DistanceKM <= 0 ||
                (request.ElevationGain.HasValue && request.ElevationGain.Value < 0))
            {
                return BadRequest(new
                {
                    message = "Provide a valid event, route name, positive distance and non-negative elevation."
                });
            }

            if (!string.IsNullOrWhiteSpace(request.RouteMapUrl) &&
                (!Uri.TryCreate(request.RouteMapUrl, UriKind.Absolute, out var mapUri) ||
                 (mapUri.Scheme != Uri.UriSchemeHttp &&
                  mapUri.Scheme != Uri.UriSchemeHttps)))
            {
                return BadRequest(new
                {
                    message = "RouteMapUrl must be a valid HTTP or HTTPS URL."
                });
            }

            var organiserId = GetUserId();

            if (organiserId == null)
            {
                return Unauthorized(new { message = "User identity could not be verified." });
            }

            var route = await _context.Routes
                .FirstOrDefaultAsync(r => r.RouteID == id);

            if (route == null)
            {
                return NotFound(new { message = "Route not found." });
            }

            var existingEvent = await _context.Events
                .FirstOrDefaultAsync(e => e.EventID == route.EventID);

            if (existingEvent == null)
            {
                return NotFound(new { message = "The route's event was not found." });
            }

            if (existingEvent.OrganiserID != organiserId.Value)
            {
                return Forbid();
            }

            var targetEvent = await _context.Events
                .FirstOrDefaultAsync(e => e.EventID == request.EventID);

            if (targetEvent == null)
            {
                return NotFound(new { message = "Target event not found." });
            }

            if (targetEvent.OrganiserID != organiserId.Value)
            {
                return Forbid();
            }

            route.EventID = request.EventID;
            route.RouteName = request.RouteName.Trim();
            route.DistanceKM = request.DistanceKM;
            route.ElevationGain = request.ElevationGain;
            route.RouteMapUrl = request.RouteMapUrl;
            route.Description = request.Description;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Route updated successfully.",
                route
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> DeleteRoute(int id)
        {
            var organiserId = GetUserId();

            if (organiserId == null)
            {
                return Unauthorized(new { message = "User identity could not be verified." });
            }

            var route = await _context.Routes
                .FirstOrDefaultAsync(r => r.RouteID == id);

            if (route == null)
            {
                return NotFound(new { message = "Route not found." });
            }

            var eventItem = await _context.Events
                .FirstOrDefaultAsync(e => e.EventID == route.EventID);

            if (eventItem == null)
            {
                return NotFound(new { message = "The route's event was not found." });
            }

            if (eventItem.OrganiserID != organiserId.Value)
            {
                return Forbid();
            }

            _context.Routes.Remove(route);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Route deleted successfully." });
        }

        private int? GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (int.TryParse(userId, out var id))
            {
                return id;
            }

            return null;
        }
    }
}
