using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LibRes.App.Models.Calendar;

namespace LibRes.App.Models.Account
{
    public class ProfileViewModel
    {
        [Display(Name = "First Name")] public string FirstName { get; set; }

        [Display(Name = "Last Name")] public string LastName { get; set; }

        [Display(Name = "Email")] public string Email { get; set; }

        [Display(Name = "Phone Number")] public string PhoneNumber { get; set; }

        public List<EventSingleView> Events { get; set; }
    }
}