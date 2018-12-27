using System.ComponentModel.DataAnnotations;

namespace LibRes.App.Models.Account
{
    public class ProfileEditModel
    {
        [Display(Name = "Fisrt Name")]
        [MaxLength(30)]
        public string FirstName { get; set; }
        
        [Display(Name = "Last Name")]
        [MaxLength(30)]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }
}