
namespace RaceDay.Mvc.ViewModels
{
    public class EnrolmentViewModel
    {
        public int EnrolmentID { get; set; }

        public int UserID { get; set; }

        public string ParticipantName { get; set; } = string.Empty;

        public int EventCategoryID { get; set; }

        public string EventName { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;

        public string PaymentStatus { get; set; } = string.Empty;

        public string EnrolmentStatus { get; set; } = string.Empty;
    }
}
