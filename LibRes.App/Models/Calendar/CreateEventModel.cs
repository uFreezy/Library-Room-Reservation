using System;
using System.ComponentModel.DataAnnotations;
using LibRes.App.Models.Calendar;

namespace LibRes.App.Models.Calendar
{
    public class CreateEventModel
    {
        [Required]
        [Display(Name = "Name")]
        public string EventName {get; set;}

        [Required]
        [Display(Name = "Beginning")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        public DateTime BeginHour { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Ending")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        public DateTime EndHour { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date")]
        public DateTime EventDate { get; set; }

        public EventRepeatViewModel EventRepeatModel { get; set; }


        // TODO: Remove
        public bool IsReoccuring { get; set; }

        [Required]
        [Display(Name = "Room")]
        public string MeetingRoomId { get; set; }

        [Required]
        [Display(Name = "Department")]
        public string Department { get; set; }

        [Required]
        [Display(Name = "Need Multimedia ?")]
        public bool WantsMultimedia { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

    }
}
