using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibRes.App.DbModels
{
    public class EventOccurenceModel
    {
        public EventOccurenceModel()
        {
        }

        public EventOccurenceModel(ReservationModel reservation, DateTime occurence, double durationMinutes)
        {
            Reservation = reservation;
            Occurence = occurence;
            DurationMinutes = durationMinutes;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required] public virtual ReservationModel Reservation { get; set; }

        [Required] public DateTime Occurence { get; set; }

        [Required] public double DurationMinutes { get; set; }
    }
}