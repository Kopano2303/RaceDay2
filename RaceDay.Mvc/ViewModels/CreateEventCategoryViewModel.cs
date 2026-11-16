
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.ViewModels
{
    public class CreateEventCategoryViewModel
    {
        [Required]
        public int EventID { get; set; }

        [Required]
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Entry fee is required.")]
        [Range(0, 100000, ErrorMessage = "Enter a valid entry fee.")]
        public decimal EntryFee { get; set; }

        [Range(1, 100000, ErrorMessage = "Maximum participants must be at least 1.")]
        public int? MaximumParticipants { get; set; }

        public List<EventViewModel> Events { get; set; }
            = new List<EventViewModel>();

        public List<CategoryViewModel> Categories { get; set; }
            = new List<CategoryViewModel>();
    }
}
