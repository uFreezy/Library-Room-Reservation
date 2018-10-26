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
        public virtual ICollection<EventOccuranceModel> EventDates 
        { 
            get { return this._eventDates; }
            set { this._eventDates = value; }
        }

        // TODO: Add regex validation to be sure that value is actually time.
        //[RegularExpression(@"(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]")]
        [Required]
        public string BeginHour { get; set; }

        // TODO: Add regex validation to be sure that value is actually time.
        //[RegularExpression(@"^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$")]
        [Required]
        public string EndHour { get; set; }

        [Required]
        public RoomModel MeetingRoom { get; set; }

        [Required]
        public string Department { get; set; }

        [Required]
        public ApplicationUser ReservationOwner { get; set; }

        [Required]
        public bool WantsMultimedia { get; set; }

        public string Description { get; set; }
    }
}
