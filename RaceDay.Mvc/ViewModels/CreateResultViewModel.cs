
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.ViewModels
{
    public class CreateResultViewModel
    {
        [Required(ErrorMessage = "Please select an enrolment.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid enrolment.")]
        public int EnrolmentID { get; set; }

        [Required(ErrorMessage = "Finish time is required.")]
        public TimeSpan FinishTime { get; set; }

        [Range(1, 100000, ErrorMessage = "Overall position must be at least 1.")]
        public int? OverallPosition { get; set; }

        [Range(1, 100000, ErrorMessage = "Category position must be at least 1.")]
        public int? CategoryPosition { get; set; }

        [Required(ErrorMessage = "Result status is required.")]
        public string Status { get; set; } = "Finished";

        public List<EnrolmentViewModel> Enrolments { get; set; }
            = new List<EnrolmentViewModel>();
    }
}
