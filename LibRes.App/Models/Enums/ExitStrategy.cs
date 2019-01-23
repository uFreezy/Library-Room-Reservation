using System.ComponentModel.DataAnnotations;

namespace LibRes.App.Models.Enums
{
    public enum ExitStrategy
    {
        [Display(Name = "Never")]
        NEVER = 1,
        [Display(Name = "Fixed Date")]
        FIXED_DATE = 2
    }
}