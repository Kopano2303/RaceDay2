
using System;
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.ViewModels
{
    public class ProfileViewModel
    {
        public int UserID { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        public string? Gender { get; set; }

        public string? EmergencyContactName { get; set; }

        [Phone(ErrorMessage = "Enter a valid emergency contact number.")]
        public string? EmergencyContactPhone { get; set; }

        public string? ProfileImageUrl { get; set; }
    }
}
