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
            _reservations = new HashSet<ReservationModel>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required] public string RoomName { get; set; }

        public virtual ICollection<ReservationModel> RoomReservations
        {
            get => _reservations;
            set => _reservations = value;
        }    
        
        // TODO : Add color 
    }
}