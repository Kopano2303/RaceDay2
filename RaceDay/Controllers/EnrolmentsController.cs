using Microsoft.AspNetCore.Mvc;
using RaceDay.DTOs;

namespace RaceDay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrolmentsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetEnrolments()
        {
            return Ok(new
            {
                message = "Get all enrolments endpoint is working."
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetEnrolment(int id)
        {
            return Ok(new
            {
                message = "Get enrolment endpoint is working.",
                enrolmentId = id
            });
        }

        [HttpGet("user/{userId}")]
        public IActionResult GetUserEnrolments(int userId)
        {
            return Ok(new
            {
                message = "Get user enrolments endpoint is working.",
                userId = userId
            });
        }

        [HttpPost]
        public IActionResult CreateEnrolment(CreateEnrolmentRequest request)
        {
            return Ok(new
            {
                message = "Create enrolment endpoint is working.",
                data = request
            });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateEnrolment(int id, CreateEnrolmentRequest request)
        {
            return Ok(new
            {
                message = "Update enrolment endpoint is working.",
                enrolmentId = id,
                data = request
            });
        }

        [HttpDelete("{id}")]
        public IActionResult CancelEnrolment(int id)
        {
            return Ok(new
            {
                message = "Cancel enrolment endpoint is working.",
                enrolmentId = id
            });
        }
    }
}
