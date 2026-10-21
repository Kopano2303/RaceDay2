
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Controllers;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Models;
using Xunit;

namespace RaceDay.Tests
{
    public class EventsControllerTests
    {
        private readonly RaceDayContext _context;
        private readonly EventsController _controller;

        public EventsControllerTests()
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

            _controller = new EventsController(_context);
        }

        [Fact]
        public async Task GetEvents_ReturnsOk()
        {
            var result = await _controller.GetEvents();

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetEvent_MissingEvent_ReturnsNotFound()
        {
            var result = await _controller.GetEvent(999);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task CreateEvent_InvalidEventName_ReturnsBadRequest()
        {
            var request = new CreateEventRequest
            {
                OrganiserID = 1,
                EventName = "",
                Description = "Test event",
                EventDate = DateTime.Today.AddDays(30),
                StartTime = new TimeSpan(8, 0, 0),
                Venue = "Test Stadium",
                City = "Pretoria",
                Province = "Gauteng",
                Status = "Upcoming",
                EventType = "Run"
            };

            var result = await _controller.CreateEvent(request);

            Assert.IsType<BadRequestObjectResult>(result);
            Assert.Empty(_context.Events);
        }
    }
}
