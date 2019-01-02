using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LibRes.App.DbModels;

namespace LibRes.App.Models.Calendar
{
    public class EventEditModel
    {
        [Required]
        public int Id { get; set; }
        
        [Required]
        [Display(Name = "Name")]
        public string EventName { get; set; }

        [Required] 
        [Display(Name = "Room")] 
        public string MeetingRoomId { get; set; }

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
        public string Department { get; set; }

        [Display(Name = "Multimedia ?")]
        public bool WantsMultimedia { get; set; }

        public string Description { get; set; }

        [Display(Name = "Apply for all future occurrences")]
        public bool ShouldApplyForAllDates { get; set; }
    }
}