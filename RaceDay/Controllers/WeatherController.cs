using Microsoft.AspNetCore.Mvc;

namespace RaceDay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetWeather()
        {
            return Ok(new
            {
                message = "Get all weather records endpoint is working."
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetWeatherRecord(int id)
        {
            return Ok(new
            {
                message = "Get weather record endpoint is working.",
                weatherId = id
            });
        }

        [HttpGet("event/{eventId}")]
        public IActionResult GetEventWeather(int eventId)
        {
            return Ok(new
            {
                message = "Get event weather endpoint is working.",
                eventId = eventId
            });
        }
    }
}