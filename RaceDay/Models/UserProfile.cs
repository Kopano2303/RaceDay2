namespace RaceDay.Models
{
    public class UserProfile
    {
        public int ProfileID { get; set; }

        public int UserID { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? Gender { get; set; }

        public string? EmergencyContactName { get; set; }

        public string? EmergencyContactPhone { get; set; }

        public string? ProfileImageUrl { get; set; }

        public User? User { get; set; }
    }
}