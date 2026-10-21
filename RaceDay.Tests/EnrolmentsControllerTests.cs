
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Controllers;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Models;
using Xunit;

namespace RaceDay.Tests
{
    public class EnrolmentsControllerTests
    {
        private readonly RaceDayContext _context;
        private readonly EnrolmentsController _controller;

        public EnrolmentsControllerTests()
        {
            var options = new DbContextOptionsBuilder<RaceDayContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new RaceDayContext(options);
            _context.Database.EnsureCreated();

            _context.Roles.AddRange(
                new Role { RoleID = 1, RoleName = "Organiser" },
                new Role { RoleID = 2, RoleName = "Participant" });

            _context.SaveChanges();

            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "10"),
                        new Claim(ClaimTypes.Role, "Participant")
                    },
                    "TestAuthentication"));

            _controller = new EnrolmentsController(_context)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = user
                    }
                }
            };
        }

        private async Task<int> AddEventCategory()
        {
            var organiser = new User
            {
                UserID = 1,
                FirstName = "Test",
                LastName = "Organiser",
                Email = "organiser@example.com",
                PasswordHash = "test-hash",
                RoleID = 1
            };

            var participant = new User
            {
                UserID = 10,
                FirstName = "Test",
                LastName = "Participant",
                Email = "participant@example.com",
                PasswordHash = "test-hash",
                RoleID = 2
            };

            _context.Users.AddRange(organiser, participant);

            var category = new Category
            {
                CategoryID = 1,
                CategoryName = "10 KM Run",
                DistanceKM = 10m,
                CategoryType = "Run"
            };

            _context.Categories.Add(category);

            var raceEvent = new RaceDay.Models.Event
            {
                EventName = "Test Race",
                OrganiserID = 1,
                EventDate = DateTime.Today.AddDays(30),
                StartTime = new TimeSpan(8, 0, 0),
                Venue = "Test Stadium",
                City = "Pretoria",
                Province = "Gauteng",
                Status = "Upcoming",
                EventType = "Run"
            };

            _context.Events.Add(raceEvent);
            await _context.SaveChangesAsync();

            var eventCategory = new EventCategory
            {
                EventID = raceEvent.EventID,
                CategoryID = category.CategoryID,
                EntryFee = 100m,
                MaximumParticipants = 100
            };

            _context.EventCategories.Add(eventCategory);
            await _context.SaveChangesAsync();

            return eventCategory.EventCategoryID;
        }

        [Fact]
        public async Task CreateEnrolment_InvalidCategoryId_ReturnsBadRequest()
        {
            var request = new CreateEnrolmentRequest
            {
                EventCategoryID = 0
            };

            var result = await _controller.CreateEnrolment(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateEnrolment_UnknownCategory_ReturnsNotFound()
        {
            var request = new CreateEnrolmentRequest
            {
                EventCategoryID = 999
            };

            var result = await _controller.CreateEnrolment(request);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task CreateEnrolment_ValidRequest_CreatesEnrolment()
        {
            int eventCategoryId = await AddEventCategory();

            var request = new CreateEnrolmentRequest
            {
                EventCategoryID = eventCategoryId
            };

            var result = await _controller.CreateEnrolment(request);

            Assert.IsType<ObjectResult>(result);
            var response = (ObjectResult)result;
            Assert.Equal(201, response.StatusCode);

            var savedEnrolment = await _context.Enrolments
                .SingleAsync();

            Assert.Equal(10, savedEnrolment.UserID);
            Assert.Equal(eventCategoryId, savedEnrolment.EventCategoryID);
            Assert.Equal("Pending", savedEnrolment.PaymentStatus);
            Assert.Equal("Confirmed", savedEnrolment.EnrolmentStatus);
        }

        [Fact]
        public async Task CreateEnrolment_DuplicateRequest_ReturnsConflict()
        {
            int eventCategoryId = await AddEventCategory();

            _context.Enrolments.Add(new Enrolment
            {
                UserID = 10,
                EventCategoryID = eventCategoryId,
                PaymentStatus = "Pending",
                EnrolmentStatus = "Confirmed"
            });

            await _context.SaveChangesAsync();

            var request = new CreateEnrolmentRequest
            {
                EventCategoryID = eventCategoryId
            };

            var result = await _controller.CreateEnrolment(request);

            Assert.IsType<ConflictObjectResult>(result);
            Assert.Single(_context.Enrolments);
        }
    }
}
