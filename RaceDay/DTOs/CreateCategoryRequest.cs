namespace RaceDay.DTOs
{
    public class CreateCategoryRequest
    {
        public string CategoryName { get; set; } = string.Empty;

        public decimal DistanceKM { get; set; }

        public string CategoryType { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}