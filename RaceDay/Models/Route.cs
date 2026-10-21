namespace RaceDay.Models
{
    public class Route
    {
        public int RouteID { get; set; }

        public int EventID { get; set; }

        public string RouteName { get; set; } = string.Empty;

        public decimal DistanceKM { get; set; }

        public int? ElevationGain { get; set; }

        public string? RouteMapUrl { get; set; }

        public string? Description { get; set; }

        public Event? Event { get; set; }
    }
}
