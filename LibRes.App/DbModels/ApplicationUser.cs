using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LibRes.App.DbModels;
using Microsoft.AspNetCore.Identity;

namespace LibRes.App.Models
{
    public class ApplicationUser : IdentityUser
    {
        private ICollection<ReservationModel> _reservations;

        public ApplicationUser()
        {
            this._reservations = new HashSet<ReservationModel>();
        }

        [Required]
        [MaxLength(30)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(30)]
        public string LastName { get; set; }

        public virtual ICollection<ReservationModel> Reservations
        {
            get { return this._reservations; }
            set { this._reservations = value; }
        }
    }
}
