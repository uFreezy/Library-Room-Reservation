using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibRes.App.DbModels
{
    public class ReservationModel
    {
        public ReservationModel()
        {
            EventDates = new HashSet<EventOccuranceModel>();
            WantsMultimedia = false;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required] public string EventName { get; set; }

        [Required] public ICollection<EventOccuranceModel> EventDates { get; set; }

        [Required] public RoomModel MeetingRoom { get; set; }

        [Required] public string Department { get; set; }

        [Required] public ApplicationUser ReservationOwner { get; set; }

        [Required] public bool WantsMultimedia { get; set; }

        public string Description { get; set; }

        [Required] public DateTime CreatedOn { get; set; }
    }
}