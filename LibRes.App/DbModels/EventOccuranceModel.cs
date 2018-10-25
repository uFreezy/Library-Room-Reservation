using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibRes.App.DbModels
{
    public class EventOccuranceModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public ReservationModel Reservation { get; set; }

        [Required]
        public DateTime Occurance { get; set; }
    }
}
