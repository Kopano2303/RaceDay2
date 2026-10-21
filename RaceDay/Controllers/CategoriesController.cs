
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

        [HttpGet("{id:int}")]
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
                return NotFound(new
                {
                    message = "Category not found."
                });
            }

            return Ok(category);
        }

        [Authorize(Roles = "Organiser")]
        [HttpPost]
        public async Task<IActionResult> CreateCategory(
            [FromBody] CreateCategoryRequest request)
        {
            string categoryName = request.CategoryName?.Trim() ?? "";
            string categoryType = request.CategoryType?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(categoryName) ||
                string.IsNullOrWhiteSpace(categoryType) ||
                request.DistanceKM <= 0)
            {
                return BadRequest(new
                {
                    message = "Category name and type are required, and distance must be greater than zero."
                });
            }

            string[] allowedTypes = { "Run", "Walk", "Cycle" };

            if (!allowedTypes.Contains(
                categoryType,
                StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest(new
                {
                    message = "Category type must be Run, Walk, or Cycle."
                });
            }

            bool exists = await _context.Categories.AnyAsync(c =>
                c.CategoryName.ToLower() == categoryName.ToLower());

            if (exists)
            {
                return Conflict(new
                {
                    message = "A category with this name already exists."
                });
            }

            var category = new Category
            {
                CategoryName = categoryName,
                DistanceKM = request.DistanceKM,
                CategoryType = allowedTypes.First(t =>
                    t.Equals(categoryType, StringComparison.OrdinalIgnoreCase)),
                Description = string.IsNullOrWhiteSpace(request.Description)
                    ? null
                    : request.Description.Trim()
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
