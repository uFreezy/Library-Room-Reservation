using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibRes.App.DbModels
{
    public class RoomModel
    {
        private ICollection<ReservationModel> _reservations;

        public RoomModel()
        {
            this._reservations = new HashSet<ReservationModel>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string RoomName { get; set; }

        public virtual ICollection<ReservationModel> RoomReservations
        {
            get { return this._reservations; }
            set { this._reservations = value; }
        }
    }
}
