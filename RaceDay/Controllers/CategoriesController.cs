
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Models;

namespace RaceDay.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly RaceDayContext _context;

        public CategoriesController(RaceDayContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .OrderBy(c => c.CategoryName)
                .Select(c => new
                {
                    c.CategoryID,
                    c.CategoryName,
                    c.DistanceKM,
                    c.CategoryType,
                    c.Description
                })
                .ToListAsync();

            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _context.Categories
                .AsNoTracking()
                .Where(c => c.CategoryID == id)
                .Select(c => new
                {
                    c.CategoryID,
                    c.CategoryName,
                    c.DistanceKM,
                    c.CategoryType,
                    c.Description
                })
                .FirstOrDefaultAsync();

            if (category == null)
            {
                return NotFound(new { message = "Category not found." });
            }

            return Ok(category);
        }

        [Authorize(Roles = "Organiser")]
        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.CategoryName) ||
                string.IsNullOrWhiteSpace(request.CategoryType) ||
                request.DistanceKM <= 0)
            {
                return BadRequest(new
                {
                    message = "Category name and type are required, and distance must be greater than zero."
                });
            }

            bool exists = await _context.Categories.AnyAsync(c =>
                c.CategoryName == request.CategoryName);

            if (exists)
            {
                return Conflict(new { message = "A category with this name already exists." });
            }

            var category = new Category
            {
                CategoryName = request.CategoryName.Trim(),
                DistanceKM = request.DistanceKM,
                CategoryType = request.CategoryType.Trim(),
                Description = request.Description
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCategory),
                new { id = category.CategoryID },
                new
                {
                    category.CategoryID,
                    category.CategoryName,
                    category.DistanceKM,
                    category.CategoryType,
                    category.Description
                });
        }
    }
}
