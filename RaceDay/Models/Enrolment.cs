namespace RaceDay.Models
{
    public class Enrolment
    {
        public int EnrolmentID { get; set; }

        public int UserID { get; set; }

        public int EventCategoryID { get; set; }

        public DateTime EnrolmentDate { get; set; }

        public string RaceNumber { get; set; } = string.Empty;

        public string PaymentStatus { get; set; } = string.Empty;

        public string EnrolmentStatus { get; set; } = string.Empty;

        public User? User { get; set; }

        public EventCategory? EventCategory { get; set; }

        public Result? Result { get; set; }
    }
}