using Microsoft.AspNetCore.Mvc;
using RaceDay.DTOs;

namespace RaceDay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventCategoriesController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetEventCategories()
        {
            return Ok(new
            {
                message = "Get all event categories endpoint is working."
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetEventCategory(int id)
        {
            return Ok(new
            {
                message = "Get event category endpoint is working.",
                eventCategoryId = id
            });
        }

        [HttpGet("event/{eventId}")]
        public IActionResult GetEventCategoriesByEvent(int eventId)
        {
            return Ok(new
            {
                message = "Get event categories by event endpoint is working.",
                eventId = eventId
            });
        }

        [HttpPost]
        public IActionResult CreateEventCategory(CreateEventCategoryRequest request)
        {
            return Ok(new
            {
                message = "Create event category endpoint is working.",
                data = request
            });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateEventCategory(
            int id,
            CreateEventCategoryRequest request)
        {
            return Ok(new
            {
                message = "Update event category endpoint is working.",
                eventCategoryId = id,
                data = request
            });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteEventCategory(int id)
        {
            return Ok(new
            {
                message = "Delete event category endpoint is working.",
                eventCategoryId = id
            });
        }
    }
}