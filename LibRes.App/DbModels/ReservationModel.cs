using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LibRes.App.Models;

namespace LibRes.App.DbModels
{
    public class ReservationModel
    {
        private ICollection<EventOccuranceModel> _eventDates;

        public ReservationModel()
        {
            this._eventDates = new HashSet<EventOccuranceModel>();
            this.WantsMultimedia = false;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string EventName { get; set; }

        [Required]
        public  ICollection<EventOccuranceModel> EventDates 
        { 
            get { return this._eventDates; }
            set { this._eventDates = value; }
        }

        [Required]
        public string BeginHour { get; set; }

        [Required]
        public string EndHour { get; set; }

        [Required]
        public  RoomModel MeetingRoom { get; set; }

        [Required]
        public string Department { get; set; }

        [Required]
        public ApplicationUser ReservationOwner { get; set; }

        [Required]
        public bool WantsMultimedia { get; set; }

        public string Description { get; set; }
    }
}
