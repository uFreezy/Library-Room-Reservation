using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace LibRes.App.DbModels
{
    public class ApplicationUser : IdentityUser
    {
        private ICollection<ReservationModel> _reservations;
        private string _secretAnswer;

        public ApplicationUser()
        {
            _reservations = new HashSet<ReservationModel>();
        }

        [Required] [MaxLength(30)] public string FirstName { get; set; }

        [Required] [MaxLength(30)] public string LastName { get; set; }

        [Required] [MaxLength(100)] public string SecretQuestion { get; set; }

        [Required]
        [MaxLength(100)]
        // TODO: Find a way to encrypt and decrypt it so we don't keep it as a plain string.
        public string SecretAnswer
        {
            get => _secretAnswer;
            set => _secretAnswer = EncryptionUtil.Encrypt(value);
        }

        public virtual ICollection<ReservationModel> Reservations
        {
            get => _reservations;
            set => _reservations = value;
        }

        /*
         * This here is needed because when Entity framework puts data in the DB
         * it calls the DB model's getter, so the getter has to return a encryped
         * value as well otherwise there isn't a point to the whole encryption.
         * So with this non-mapped property we can actually have a way to fetch the 
         * non-encrypted value.
        */
        [NotMapped]
        public string SecrectAnswerDecrypted
        {
            get => EncryptionUtil.Decrypt(SecretAnswer);
            private set => _secretAnswer = EncryptionUtil.Encrypt(value);
        }
    }
}