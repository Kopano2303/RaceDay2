using Microsoft.AspNetCore.Mvc;
using RaceDay.DTOs;

namespace RaceDay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoutesController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetRoutes()
        {
            return Ok(new
            {
                message = "Get all routes endpoint is working."
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetRoute(int id)
        {
            return Ok(new
            {
                message = "Get route endpoint is working.",
                routeId = id
            });
        }

        [HttpGet("event/{eventId}")]
        public IActionResult GetRouteByEvent(int eventId)
        {
            return Ok(new
            {
                message = "Get route by event endpoint is working.",
                eventId = eventId
            });
        }

        [HttpPost]
        public IActionResult CreateRoute(CreateRouteRequest request)
        {
            return Ok(new
            {
                message = "Create route endpoint is working.",
                data = request
            });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateRoute(int id, CreateRouteRequest request)
        {
            return Ok(new
            {
                message = "Update route endpoint is working.",
                routeId = id,
                data = request
            });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteRoute(int id)
        {
            return Ok(new
            {
                message = "Delete route endpoint is working.",
                routeId = id
            });
        }
    }
}
