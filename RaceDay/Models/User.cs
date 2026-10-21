namespace RaceDay.Models
{
    public class User
    {
        public int UserID { get; set; }

        public int RoleID { get; set; }

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public DateTime CreatedAt { get; set; }

        public Role? Role { get; set; }

        public UserProfile? UserProfile { get; set; }

        public ICollection<Event> OrganisedEvents { get; set; } = new List<Event>();

        public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
    }
}