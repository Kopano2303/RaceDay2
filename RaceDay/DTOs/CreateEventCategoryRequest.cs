namespace RaceDay.DTOs
{
    public class CreateEventCategoryRequest
    {
        public int EventID { get; set; }

        public int CategoryID { get; set; }

        public decimal EntryFee { get; set; }

        public int? MaximumParticipants { get; set; }
    }
}