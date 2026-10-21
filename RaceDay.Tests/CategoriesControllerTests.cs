
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Controllers;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Models;
using Xunit;

namespace RaceDay.Tests
{
    public class CategoriesControllerTests
    {
        private readonly RaceDayContext _context;
        private readonly CategoriesController _controller;

        public CategoriesControllerTests()
        {
            var options = new DbContextOptionsBuilder<RaceDayContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new RaceDayContext(options);
            _context.Database.EnsureCreated();

            _controller = new CategoriesController(_context);
        }

        [Fact]
        public async Task GetCategories_ReturnsAllCategories()
        {
            _context.Categories.AddRange(
                new Category
                {
                    CategoryName = "Half Marathon",
                    DistanceKM = 21.1m,
                    CategoryType = "Run"
                },
                new Category
                {
                    CategoryName = "Fun Walk",
                    DistanceKM = 5m,
                    CategoryType = "Walk"
                });

            await _context.SaveChangesAsync();

            var result = await _controller.GetCategories();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var categories = Assert.IsAssignableFrom<System.Collections.IEnumerable>(
                okResult.Value);

            Assert.NotNull(categories);
            Assert.Equal(2, _context.Categories.Count());
        }

        [Fact]
        public async Task GetCategory_ExistingCategory_ReturnsOk()
        {
            var category = new Category
            {
                CategoryName = "Marathon",
                DistanceKM = 42.195m,
                CategoryType = "Run"
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var result = await _controller.GetCategory(category.CategoryID);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetCategory_MissingCategory_ReturnsNotFound()
        {
            var result = await _controller.GetCategory(999);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task CreateCategory_ValidRequest_CreatesCategory()
        {
            var request = new CreateCategoryRequest
            {
                CategoryName = "Trail Run",
                DistanceKM = 10m,
                CategoryType = "Run",
                Description = "A trail running event"
            };

            var result = await _controller.CreateCategory(request);

            Assert.IsType<CreatedAtActionResult>(result);

            var savedCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryName == "Trail Run");

            Assert.NotNull(savedCategory);
            Assert.Equal(10m, savedCategory.DistanceKM);
            Assert.Equal("Run", savedCategory.CategoryType);
        }

        [Fact]
        public async Task CreateCategory_ZeroDistance_ReturnsBadRequest()
        {
            var request = new CreateCategoryRequest
            {
                CategoryName = "Invalid Category",
                DistanceKM = 0,
                CategoryType = "Run"
            };

            var result = await _controller.CreateCategory(request);

            Assert.IsType<BadRequestObjectResult>(result);
            Assert.Empty(_context.Categories);
        }

        [Fact]
        public async Task CreateCategory_InvalidType_ReturnsBadRequest()
        {
            var request = new CreateCategoryRequest
            {
                CategoryName = "Swimming",
                DistanceKM = 2m,
                CategoryType = "Swim"
            };

            var result = await _controller.CreateCategory(request);

            Assert.IsType<BadRequestObjectResult>(result);
            Assert.Empty(_context.Categories);
        }

        [Fact]
        public async Task CreateCategory_DuplicateName_ReturnsConflict()
        {
            _context.Categories.Add(new Category
            {
                CategoryName = "Marathon",
                DistanceKM = 42.195m,
                CategoryType = "Run"
            });

            await _context.SaveChangesAsync();

            var request = new CreateCategoryRequest
            {
                CategoryName = " marathon ",
                DistanceKM = 10m,
                CategoryType = "Run"
            };

            var result = await _controller.CreateCategory(request);

            Assert.IsType<ConflictObjectResult>(result);
            Assert.Single(_context.Categories);
        }
    }
}
