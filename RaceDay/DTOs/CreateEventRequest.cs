namespace RaceDay.DTOs
{
    public class CreateEventRequest
    {
        public int OrganiserID { get; set; }

        public string EventName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string EventType { get; set; } = string.Empty;

        public DateTime EventDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public string Venue { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string Province { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
    }
}