namespace RaceDay.DTOs
{
    public class UpdateProfileRequest
    {
        public DateTime? DateOfBirth { get; set; }

        public string? Gender { get; set; }

        public string? EmergencyContactName { get; set; }

        public string? EmergencyContactPhone { get; set; }

        public string? ProfileImageUrl { get; set; }
    }
}