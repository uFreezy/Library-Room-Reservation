using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibRes.App.DbModels
{
    public class EventOccuranceModel
    {
        public EventOccuranceModel()
        {
        }

        public EventOccuranceModel(ReservationModel reservation, DateTime occurance)
        {
            Reservation = reservation;
            Occurance = occurance;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required] public virtual ReservationModel Reservation { get; set; }

        [Required] public DateTime Occurance { get; set; }
    }
}