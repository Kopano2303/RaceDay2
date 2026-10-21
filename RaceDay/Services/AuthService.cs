
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Models;

namespace RaceDay.Services
{
    public class AuthService
    {
        private readonly RaceDayContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthService(RaceDayContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<object> Register(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password) ||
                string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.LastName))
            {
                throw new ArgumentException("Email, password, first name and last name are required.");
            }

            string email = request.Email.Trim().ToLowerInvariant();

            if (await _context.Users.AnyAsync(u => u.Email == email))
            {
                throw new ArgumentException("An account with this email already exists.");
            }

            string roleName = request.RoleName.Trim();

            if (!roleName.Equals("Organiser", StringComparison.OrdinalIgnoreCase) &&
                !roleName.Equals("Participant", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Role must be Organiser or Participant.");
            }

            Role? role = await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleName == roleName);

            if (role == null)
            {
                throw new InvalidOperationException(
                    "The selected role is not configured in the database. Add the Organiser and Participant roles first.");
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

            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

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
                throw new ArgumentException("Email and password are required.");
            }

            string email = request.Email.Trim().ToLowerInvariant();

            User? user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            PasswordVerificationResult result = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            return new
            {
                message = "Login successful.",
                user.UserID,
                user.Email,
                user.FirstName,
                user.LastName,
                role = user.Role?.RoleName
            };
        }
    }
}
