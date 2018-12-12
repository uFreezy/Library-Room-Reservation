using System;
using System.Collections.Generic;
using LibRes.App.DbModels;

namespace LibRes.App.Models.Account
{
    public class ProfileViewModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public List<ReservationModel> Reservations { get; set; }
    }
}