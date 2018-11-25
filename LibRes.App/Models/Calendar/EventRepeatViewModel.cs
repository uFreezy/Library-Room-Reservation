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

        [Required] public List<DaysOfWeekEnumModel> DaysOfTheWeek { get; set; }

        [Required] [Display(Name = "End :")] public ExitStrategy ExitStrategy { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime ExitDate { get; set; }
    }
}