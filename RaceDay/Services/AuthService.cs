using RaceDay.DTOs;

namespace RaceDay.Services
{
    public class AuthService
    {
        public object Register(RegisterRequest request)
        {
            return new
            {
                message = "User registration successful.",
                email = request.Email,
                firstName = request.FirstName,
                lastName = request.LastName
            };
        }

        public object Login(LoginRequest request)
        {
            return new
            {
                message = "Login successful.",
                email = request.Email,
                token = "temporary-token"
            };
        }
    }
}