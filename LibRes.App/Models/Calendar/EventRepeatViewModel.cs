using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LibRes.App.Models.Enums;

namespace LibRes.App.Models.Calendar
{
    public class EventRepeatViewModel
    {
        [Required]
        [Display(Name = "Repeats: ")]
        public EventRepeatOptions RepeatOption { get; set; }

        [Required(ErrorMessage = "Repeat interval is required.")]
        [Display(Name = "Repeats every: ")]
        public int RepeatInterval { get; set; }

        [Display(Name = "Days To Repeat On: ")]
        [Required] public List<DaysOfWeekEnumModel> DaysOfTheWeek { get; set; }

        [Required] [Display(Name = "End Condition:")] public ExitStrategy ExitStrategy { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        
        [Display(Name = "Exit Date")]
        public DateTime ExitDate { get; set; }
    }
}