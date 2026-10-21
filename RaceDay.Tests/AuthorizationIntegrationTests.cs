
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Net.Http.Json;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace RaceDay.Tests
{
	public class AuthorizationIntegrationTests
		: IClassFixture<CustomWebApplicationFactory>
	{
		private readonly HttpClient _client;

		public AuthorizationIntegrationTests(
			CustomWebApplicationFactory factory)
		{
			_client = factory.CreateClient();
		}

		private static string CreateToken(string role)
		{
			var claims = new[]
			{
				new Claim(ClaimTypes.NameIdentifier, "1"),
				new Claim(ClaimTypes.Name, "Integration Test User"),
				new Claim(ClaimTypes.Email, "integration@test.com"),
				new Claim(ClaimTypes.Role, role)
			};

			var key = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(
					CustomWebApplicationFactory.TestJwtKey));

			var credentials = new SigningCredentials(
				key,
				SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(10),
				signingCredentials: credentials);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		[Fact]
		public async Task Categories_WithoutToken_ReturnsUnauthorized()
		{
			var response = await _client.GetAsync("/api/Categories");

			Assert.Equal(
				HttpStatusCode.Unauthorized,
				response.StatusCode);
		}

		[Fact]
		public async Task CreateCategory_ParticipantRole_ReturnsForbidden()
		{
			var token = CreateToken("Participant");

			_client.DefaultRequestHeaders.Authorization =
				new AuthenticationHeaderValue("Bearer", token);

			var request = new
			{
				categoryName = "Integration Test Category",
				distanceKM = 5m,
				categoryType = "Run",
				description = "Test category"
			};

			var response = await _client.PostAsJsonAsync(
				"/api/Categories",
				request);

			Assert.Equal(
				HttpStatusCode.Forbidden,
				response.StatusCode);
		}
	}
}
