
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Services;
using Xunit;

namespace RaceDay.Tests
{
    public class AuthServiceTests
    {
        private readonly RaceDayContext _context;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            var options = new DbContextOptionsBuilder<RaceDayContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new RaceDayContext(options);
            _context.Database.EnsureCreated();

            var settings = new Dictionary<string, string?>
            {
                {
                    "Jwt:Key",
                    "RaceDayAutomatedTestsSecretKey123456789"
                }
            };

            IConfiguration configuration =
                new ConfigurationBuilder()
                    .AddInMemoryCollection(settings)
                    .Build();

            _authService = new AuthService(_context, configuration);
        }

        [Fact]
        public async Task Register_ValidDetails_CreatesParticipant()
        {
            var request = new RegisterRequest
            {
                Email = "participant@example.com",
                Password = "RaceDayTest123!",
                FirstName = "Test",
                LastName = "Participant",
                PhoneNumber = "0712345678"
            };

            await _authService.Register(request);

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(
                    u => u.Email == "participant@example.com");

            Assert.NotNull(user);
            Assert.Equal("Participant", user.Role!.RoleName);
            Assert.NotEqual("RaceDayTest123!", user.PasswordHash);
            Assert.Equal("Test", user.FirstName);
        }

        [Fact]
        public async Task Register_DuplicateEmail_ThrowsException()
        {
            var request = new RegisterRequest
            {
                Email = "duplicate@example.com",
                Password = "RaceDayTest123!",
                FirstName = "First",
                LastName = "User"
            };

            await _authService.Register(request);

            await Assert.ThrowsAsync<ArgumentException>(
                async () => await _authService.Register(request));
        }

        [Fact]
        public async Task Register_PasswordTooShort_ThrowsException()
        {
            var request = new RegisterRequest
            {
                Email = "short@example.com",
                Password = "123",
                FirstName = "Test",
                LastName = "User"
            };

            await Assert.ThrowsAsync<ArgumentException>(
                async () => await _authService.Register(request));
        }

        [Fact]
        public async Task Register_MissingEmail_ThrowsException()
        {
            var request = new RegisterRequest
            {
                Email = "",
                Password = "RaceDayTest123!",
                FirstName = "Test",
                LastName = "User"
            };

            await Assert.ThrowsAsync<ArgumentException>(
                async () => await _authService.Register(request));
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            var registerRequest = new RegisterRequest
            {
                Email = "login@example.com",
                Password = "RaceDayTest123!",
                FirstName = "Login",
                LastName = "Tester"
            };

            await _authService.Register(registerRequest);

            var loginRequest = new LoginRequest
            {
                Email = "login@example.com",
                Password = "RaceDayTest123!"
            };

            var result = await _authService.Login(loginRequest);

            var tokenProperty = result.GetType()
                .GetProperty("token");

            var token = tokenProperty?.GetValue(result) as string;

            Assert.NotNull(token);
            Assert.NotEmpty(token);
        }

        [Fact]
        public async Task Login_WrongPassword_ThrowsException()
        {
            var registerRequest = new RegisterRequest
            {
                Email = "wrongpassword@example.com",
                Password = "RaceDayTest123!",
                FirstName = "Test",
                LastName = "User"
            };

            await _authService.Register(registerRequest);

            var loginRequest = new LoginRequest
            {
                Email = "wrongpassword@example.com",
                Password = "WrongPassword123!"
            };

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                async () => await _authService.Login(loginRequest));
        }

        [Fact]
        public async Task Login_UnknownEmail_ThrowsException()
        {
            var loginRequest = new LoginRequest
            {
                Email = "unknown@example.com",
                Password = "RaceDayTest123!"
            };

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                async () => await _authService.Login(loginRequest));
        }
    }
}
