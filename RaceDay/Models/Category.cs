namespace RaceDay.Models
{
    public class Category
    {
        public int CategoryID { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public decimal DistanceKM { get; set; }

        public string CategoryType { get; set; } = string.Empty;

        public string? Description { get; set; }

        public ICollection<EventCategory> EventCategories { get; set; } =
            new List<EventCategory>();
    }
}
