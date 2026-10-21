
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
    public class RoutesControllerTests
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
                Description = "Integration test race",
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

        private RoutesController CreateController(
            RaceDayContext context,
            int userId = 1,
            string role = "Organiser")
        {
            var controller = new RoutesController(context);

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

        private CreateRouteRequest ValidRequest()
        {
            return new CreateRouteRequest
            {
                EventID = 1,
                RouteName = "Main Route",
                DistanceKM = 10,
                ElevationGain = 100,
                RouteMapUrl = "https://example.com/route",
                Description = "Test route"
            };
        }

        [Fact]
        public async Task GetRoutes_ReturnsOk()
        {
            using var context = CreateContext();
            var controller = CreateController(context);

            var result = await controller.GetRoutes();

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetRoute_MissingRoute_ReturnsNotFound()
        {
            using var context = CreateContext();
            var controller = CreateController(context);

            var result = await controller.GetRoute(999);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetRouteByEvent_MissingEvent_ReturnsNotFound()
        {
            using var context = CreateContext();
            var controller = CreateController(context);

            var result = await controller.GetRouteByEvent(999);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task CreateRoute_InvalidDistance_ReturnsBadRequest()
        {
            using var context = CreateContext();
            var controller = CreateController(context);
            var request = ValidRequest();
            request.DistanceKM = 0;

            var result = await controller.CreateRoute(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateRoute_InvalidUrl_ReturnsBadRequest()
        {
            using var context = CreateContext();
            var controller = CreateController(context);
            var request = ValidRequest();
            request.RouteMapUrl = "not-a-valid-url";

            var result = await controller.CreateRoute(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateRoute_UnknownEvent_ReturnsNotFound()
        {
            using var context = CreateContext();
            var controller = CreateController(context);
            var request = ValidRequest();
            request.EventID = 999;

            var result = await controller.CreateRoute(request);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task CreateRoute_ValidRequest_CreatesRoute()
        {
            using var context = CreateContext();
            var controller = CreateController(context);
            var request = ValidRequest();

            var result = await controller.CreateRoute(request);

            Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(1, await context.Routes.CountAsync());
        }

        [Fact]
        public async Task CreateRoute_WrongOrganiser_ReturnsForbid()
        {
            using var context = CreateContext();
            var controller = CreateController(context, 2, "Organiser");
            var request = ValidRequest();

            var result = await controller.CreateRoute(request);

            Assert.IsType<ForbidResult>(result);
            Assert.Equal(0, await context.Routes.CountAsync());
        }

        [Fact]
        public async Task UpdateRoute_MissingRoute_ReturnsNotFound()
        {
            using var context = CreateContext();
            var controller = CreateController(context);

            var result = await controller.UpdateRoute(999, ValidRequest());

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteRoute_MissingRoute_ReturnsNotFound()
        {
            using var context = CreateContext();
            var controller = CreateController(context);

            var result = await controller.DeleteRoute(999);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteRoute_ValidRoute_DeletesRoute()
        {
            using var context = CreateContext();

            context.Routes.Add(new RaceDay.Models.Route
            {
                RouteID = 1,
                EventID = 1,
                RouteName = "Main Route",
                DistanceKM = 10,
                ElevationGain = 100,
                Description = "Test route"
            });

            await context.SaveChangesAsync();

            var controller = CreateController(context);

            var result = await controller.DeleteRoute(1);

            Assert.IsType<OkObjectResult>(result);
            Assert.Equal(0, await context.Routes.CountAsync());
        }
    }
}
