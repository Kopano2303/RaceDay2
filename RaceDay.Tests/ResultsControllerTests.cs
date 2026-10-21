
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
    public class ResultsControllerTests
    {
        private readonly RaceDayContext _context;
        private readonly ResultsController _controller;

        public ResultsControllerTests()
        {
            var options = new DbContextOptionsBuilder<RaceDayContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new RaceDayContext(options);
            _context.Database.EnsureCreated();

            _context.Roles.Add(new Role
            {
                RoleID = 1,
                RoleName = "Organiser"
            });

            _context.SaveChanges();

            var organiser = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "1"),
                        new Claim(ClaimTypes.Role, "Organiser")
                    },
                    "TestAuthentication"));

            _controller = new ResultsController(_context)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = organiser
                    }
                }
            };
        }

        private async Task<int> AddEnrolment()
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
                RoleID = 1
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

            var enrolment = new Enrolment
            {
                UserID = participant.UserID,
                EventCategoryID = eventCategory.EventCategoryID,
                PaymentStatus = "Pending",
                EnrolmentStatus = "Confirmed"
            };

            _context.Enrolments.Add(enrolment);
            await _context.SaveChangesAsync();

            return enrolment.EnrolmentID;
        }

        [Fact]
        public async Task CreateResult_InvalidEnrolmentId_ReturnsBadRequest()
        {
            var request = new CreateResultRequest
            {
                EnrolmentID = 0,
                FinishTime = TimeSpan.FromHours(1),
                Status = "Finished"
            };

            var result = await _controller.CreateResult(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateResult_UnknownEnrolment_ReturnsNotFound()
        {
            var request = new CreateResultRequest
            {
                EnrolmentID = 999,
                FinishTime = TimeSpan.FromHours(1),
                Status = "Finished"
            };

            var result = await _controller.CreateResult(request);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task CreateResult_ValidRequest_SavesResult()
        {
            int enrolmentId = await AddEnrolment();

            var request = new CreateResultRequest
            {
                EnrolmentID = enrolmentId,
                FinishTime = TimeSpan.FromHours(1),
                OverallPosition = 1,
                CategoryPosition = 1,
                Status = "Finished"
            };

            var result = await _controller.CreateResult(request);

            var response = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, response.StatusCode);

            var savedResult = await _context.Results.SingleAsync();

            Assert.Equal(enrolmentId, savedResult.EnrolmentID);
            Assert.Equal(1, savedResult.OverallPosition);
            Assert.Equal("Finished", savedResult.Status);
        }

        [Fact]
        public async Task CreateResult_DuplicateResult_ReturnsConflict()
        {
            int enrolmentId = await AddEnrolment();

            _context.Results.Add(new Result
            {
                EnrolmentID = enrolmentId,
                FinishTime = TimeSpan.FromHours(1),
                OverallPosition = 1,
                CategoryPosition = 1,
                Status = "Finished",
                RecordedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            var request = new CreateResultRequest
            {
                EnrolmentID = enrolmentId,
                FinishTime = TimeSpan.FromHours(1),
                Status = "Finished"
            };

            var result = await _controller.CreateResult(request);

            Assert.IsType<ConflictObjectResult>(result);
            Assert.Single(_context.Results);
        }
    }
}
