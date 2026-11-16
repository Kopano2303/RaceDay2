
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.ViewModels
{
    public class CategoryViewModel
    {
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        public string CategoryName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Distance is required.")]
        [Range(0.1, 500, ErrorMessage = "Enter a valid distance.")]
        public decimal DistanceKM { get; set; }

        [Required(ErrorMessage = "Category type is required.")]
        public string CategoryType { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}
