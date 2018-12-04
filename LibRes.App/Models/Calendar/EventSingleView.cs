using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibRes.App.Models.Calendar
{
    public class EventSingleView
    {
        [Required]
        public int Id { get; set; }
        
        [Required] [Display(Name = "Name: ")] public string EventName { get; set; }

        [Required]
        [Display(Name = "Date: ")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime InitialDate { get; set; }

        [Display(Name = "Repeats On: ")] public List<DateTime> RepeatDates { get; set; }

        [Required]
        [Display(Name = "Begins: ")]
        public string BeginHour { get; set; }

        [Required] [Display(Name = "Ends: ")] public string EndHour { get; set; }

        [Required] [Display(Name = "Room: ")] public string MeetingRoom { get; set; }

        [Required]
        [Display(Name = "Department: ")]
        public string Department { get; set; }

        [Required] [Display(Name = "Owner: ")] public string ReservationOwner { get; set; }

        [Required]
        [Display(Name = "Needs Multimedia: ")]
        public bool WantsMultimedia { get; set; }

        [Display(Name = "Additional Description: ")]
        public string Description { get; set; }
    }
}