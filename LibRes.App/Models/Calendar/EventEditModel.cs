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
        
        [Display(Name = "Name")]
        public string EventName { get; set; }
        
        public DateTime EventDate { get; set; }

        public EventRepeatViewModel EventRepeatModel { get; set; }

        public bool IsReoccuring { get; set; }
        
        [Required] 
        [Display(Name = "Room")] 
        public string MeetingRoomId { get; set; }

        public ICollection<EventOccuranceModel> EventDates { get; set; }

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

        public string Department { get; set; }

        //public ApplicationUser ReservationOwner { get; set; }

        public bool WantsMultimedia { get; set; }

        public string Description { get; set; }
    }
}