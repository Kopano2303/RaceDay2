using Microsoft.AspNetCore.Mvc;
using RaceDay.DTOs;

namespace RaceDay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetEvents()
        {
            return Ok(new
            {
                message = "Get all events endpoint is working."
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetEvent(int id)
        {
            return Ok(new
            {
                message = "Get event endpoint is working.",
                eventId = id
            });
        }

        [HttpPost]
        public IActionResult CreateEvent(CreateEventRequest request)
        {
            return Ok(new
            {
                message = "Create event endpoint is working.",
                data = request
            });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateEvent(int id, CreateEventRequest request)
        {
            return Ok(new
            {
                message = "Update event endpoint is working.",
                eventId = id,
                data = request
            });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteEvent(int id)
        {
            return Ok(new
            {
                message = "Delete event endpoint is working.",
                eventId = id
            });
        }
    }
}