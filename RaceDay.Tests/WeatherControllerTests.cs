
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Controllers;
using RaceDay.Data;
using RaceDay.Models;
using Xunit;

namespace RaceDay.Tests
{
    public class WeatherControllerTests
    {
        private RaceDayContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<RaceDayContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new RaceDayContext(options);

            context.Roles.AddRange(
                new Role { RoleID = 1, RoleName = "Organiser" },
                new Role { RoleID = 2, RoleName = "Participant" });

            context.Users.AddRange(
                new User
                {
                    UserID = 1,
                    FirstName = "Test",
                    LastName = "Organiser",
                    Email = "organiser@test.com",
                    PasswordHash = "test",
                    RoleID = 1
                },
                new User
                {
                    UserID = 2,
                    FirstName = "Test",
                    LastName = "Participant",
                    Email = "participant@test.com",
                    PasswordHash = "test",
                    RoleID = 2
                });

            context.Events.Add(new RaceDay.Models.Event
            {
                EventID = 1,
                OrganiserID = 1,
                EventName = "Test Race",
                Description = "Weather test event",
                EventDate = DateTime.Today.AddDays(30),
                StartTime = new TimeSpan(8, 0, 0),
                Venue = "Test Stadium",
                City = "Johannesburg",
                Province = "Gauteng",
                Status = "Upcoming",
                EventType = "Run"
            });

            context.SaveChanges();

            return context;
        }

        private WeatherController CreateController(
            RaceDayContext context,
            int userId = 1,
            string role = "Organiser")
        {
            var controller = new WeatherController(context);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(
                        new ClaimsIdentity(claims, "Test"))
                }
            };

            return controller;
        }

        private Weather ValidWeather()
        {
            return new Weather
            {
                EventID = 1,
                Temperature = 22,
                FeelsLike = 23,
                Humidity = 60,
                WindSpeed = 12,
                WeatherCondition = "Sunny",
                RecordedAt = DateTime.UtcNow
            };
        }

        [Fact]
        public async Task GetWeather_ReturnsOk()
        {
            using var context = CreateContext();
            var controller = CreateController(context);

            var result = await controller.GetWeather();

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetWeatherRecord_MissingRecord_ReturnsNotFound()
        {
            using var context = CreateContext();
            var controller = CreateController(context);

            var result = await controller.GetWeatherRecord(999);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetEventWeather_MissingEvent_ReturnsNotFound()
        {
            using var context = CreateContext();
            var controller = CreateController(context);

            var result = await controller.GetEventWeather(999);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task CreateWeatherRecord_InvalidEventId_ReturnsBadRequest()
        {
            using var context = CreateContext();
            var controller = CreateController(context);
            var request = ValidWeather();
            request.EventID = 0;

            var result = await controller.CreateWeatherRecord(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(101)]
        public async Task CreateWeatherRecord_InvalidHumidity_ReturnsBadRequest(
            int humidity)
        {
            using var context = CreateContext();
            var controller = CreateController(context);
            var request = ValidWeather();
            request.Humidity = humidity;

            var result = await controller.CreateWeatherRecord(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateWeatherRecord_NegativeWindSpeed_ReturnsBadRequest()
        {
            using var context = CreateContext();
            var controller = CreateController(context);
            var request = ValidWeather();
            request.WindSpeed = -1;

            var result = await controller.CreateWeatherRecord(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateWeatherRecord_MissingEvent_ReturnsNotFound()
        {
            using var context = CreateContext();
            var controller = CreateController(context);
            var request = ValidWeather();
            request.EventID = 999;

            var result = await controller.CreateWeatherRecord(request);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task CreateWeatherRecord_WrongOrganiser_ReturnsForbid()
        {
            using var context = CreateContext();
            var controller = CreateController(context, 2, "Organiser");
            var request = ValidWeather();

            var result = await controller.CreateWeatherRecord(request);

            Assert.IsType<ForbidResult>(result);
            Assert.Equal(0, await context.WeatherRecords.CountAsync());
        }

        [Fact]
        public async Task CreateWeatherRecord_ValidRequest_SavesRecord()
        {
            using var context = CreateContext();
            var controller = CreateController(context);
            var request = ValidWeather();

            var result = await controller.CreateWeatherRecord(request);

            Assert.IsType<CreatedAtActionResult>(result);

            var savedRecord = await context.WeatherRecords.SingleAsync();

            Assert.Equal(1, savedRecord.EventID);
            Assert.Equal(60, savedRecord.Humidity);
            Assert.Equal(12, savedRecord.WindSpeed);
        }
    }
}
