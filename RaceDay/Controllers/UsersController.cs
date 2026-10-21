using Microsoft.AspNetCore.Mvc;
using RaceDay.DTOs;

namespace RaceDay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            return Ok(new
            {
                message = "Get user endpoint is working.",
                userId = id
            });
        }

        [HttpGet("{id}/profile")]
        public IActionResult GetProfile(int id)
        {
            return Ok(new
            {
                message = "Get profile endpoint is working.",
                userId = id
            });
        }

        [HttpPut("{id}/profile")]
        public IActionResult UpdateProfile(int id, UpdateProfileRequest request)
        {
            return Ok(new
            {
                message = "Update profile endpoint is working.",
                userId = id,
                data = request
            });
        }
    }
}