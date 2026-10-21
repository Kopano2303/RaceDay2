using Microsoft.AspNetCore.Mvc;
using RaceDay.DTOs;

namespace RaceDay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResultsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetResults()
        {
            return Ok(new
            {
                message = "Get all results endpoint is working."
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetResult(int id)
        {
            return Ok(new
            {
                message = "Get result endpoint is working.",
                resultId = id
            });
        }

        [HttpGet("enrolment/{enrolmentId}")]
        public IActionResult GetEnrolmentResult(int enrolmentId)
        {
            return Ok(new
            {
                message = "Get enrolment result endpoint is working.",
                enrolmentId = enrolmentId
            });
        }

        [HttpGet("user/{userId}")]
        public IActionResult GetUserResults(int userId)
        {
            return Ok(new
            {
                message = "Get user results endpoint is working.",
                userId = userId
            });
        }

        [HttpPost]
        public IActionResult CreateResult(CreateResultRequest request)
        {
            return Ok(new
            {
                message = "Create result endpoint is working.",
                data = request
            });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateResult(int id, CreateResultRequest request)
        {
            return Ok(new
            {
                message = "Update result endpoint is working.",
                resultId = id,
                data = request
            });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteResult(int id)
        {
            return Ok(new
            {
                message = "Delete result endpoint is working.",
                resultId = id
            });
        }
    }
}