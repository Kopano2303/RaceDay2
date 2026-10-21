namespace RaceDay.Models
{
    public class Event
    {
        public int EventID { get; set; }

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

        public DateTime CreatedAt { get; set; }

        public User? Organiser { get; set; }

        public ICollection<EventCategory> EventCategories { get; set; } =
            new List<EventCategory>();

        public Route? Route { get; set; }

        public ICollection<Weather> WeatherRecords { get; set; } =
            new List<Weather>();
    }
}