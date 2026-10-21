
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RaceDay.Services
{
    public class AuthService
    {
        private readonly RaceDayContext _context;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;

        public AuthService(
            RaceDayContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<object> Register(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password) ||
                string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.LastName))
            {
                throw new ArgumentException(
                    "Email, password, first name and last name are required.");
            }

            if (request.Password.Length < 8)
            {
                throw new ArgumentException(
                    "Password must contain at least 8 characters.");
            }

            string email = request.Email.Trim().ToLowerInvariant();

            if (await _context.Users.AnyAsync(u => u.Email == email))
            {
                throw new ArgumentException(
                    "An account with this email already exists.");
            }

            Role? role = await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleName == "Participant");

            if (role == null)
            {
                throw new InvalidOperationException(
                    "The Participant role is not configured in the database.");
            }

            User user = new User
            {
                Email = email,
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                PhoneNumber = request.PhoneNumber,
                RoleID = role.RoleID,
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash = _passwordHasher.HashPassword(
                user, request.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new
            {
                message = "User registration successful.",
                user.UserID,
                user.Email,
                user.FirstName,
                user.LastName,
                role = role.RoleName
            };
        }

        public async Task<object> Login(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                throw new ArgumentException(
                    "Email and password are required.");
            }

            string email = request.Email.Trim().ToLowerInvariant();

            User? user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                throw new UnauthorizedAccessException(
                    "Invalid email or password.");
            }

            PasswordVerificationResult result =
                _passwordHasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    request.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedAccessException(
                    "Invalid email or password.");
            }

            string roleName = user.Role?.RoleName
                ?? throw new InvalidOperationException(
                    "User role was not found.");

            string jwtKey = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException(
                    "JWT key is missing from configuration.");

            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.UserID.ToString()),

                new Claim(
                    ClaimTypes.Email,
                    user.Email),

                new Claim(
                    ClaimTypes.Name,
                    user.FirstName + " " + user.LastName),

                new Claim(
                    ClaimTypes.Role,
                    roleName)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            DateTime expiry = DateTime.UtcNow.AddHours(2);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: expiry,
                signingCredentials: credentials);

            string tokenString =
                new JwtSecurityTokenHandler().WriteToken(token);

            return new
            {
                message = "Login successful.",
                token = tokenString,
                expiresAt = expiry,
                user.UserID,
                user.Email,
                user.FirstName,
                user.LastName,
                role = roleName
            };
        }
    }
}

