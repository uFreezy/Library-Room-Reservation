using System.ComponentModel.DataAnnotations;

namespace LibRes.App.Models.Account
{
    public class ProfileEditModel
    {
        [MaxLength(30)]
        public string FirstName { get; set; }
        
        [MaxLength(30)]
        public string LastName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }
    }
}