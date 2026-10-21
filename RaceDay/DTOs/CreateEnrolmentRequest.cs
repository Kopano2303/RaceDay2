namespace RaceDay.DTOs
{
    public class CreateEnrolmentRequest
    {
        public int UserID { get; set; }

        public int EventCategoryID { get; set; }

        public string PaymentStatus { get; set; } = string.Empty;

        public string EnrolmentStatus { get; set; } = string.Empty;
    }
}