using Microsoft.AspNetCore.Mvc;
using RaceDay.DTOs;

namespace RaceDay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetCategories()
        {
            return Ok(new
            {
                message = "Get all categories endpoint is working."
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetCategory(int id)
        {
            return Ok(new
            {
                message = "Get category endpoint is working.",
                categoryId = id
            });
        }

        [HttpPost]
        public IActionResult CreateCategory(CreateCategoryRequest request)
        {
            return Ok(new
            {
                message = "Create category endpoint is working.",
                data = request
            });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCategory(int id, CreateCategoryRequest request)
        {
            return Ok(new
            {
                message = "Update category endpoint is working.",
                categoryId = id,
                data = request
            });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(int id)
        {
            return Ok(new
            {
                message = "Delete category endpoint is working.",
                categoryId = id
            });
        }
    }
}