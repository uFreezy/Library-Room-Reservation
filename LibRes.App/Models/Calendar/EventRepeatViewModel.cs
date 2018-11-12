using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LibRes.App.Models.Enums;

namespace LibRes.App.Models.Calendar
{
    public class EventRepeatViewModel
    {
        [Required]
        public EventRepeatOptions RepeatOption { get; set; }

        [Required]
        public int RepeatInterval { get; set; }

        [Required]
        public List<DaysOfWeekEnumModel> DaysOfTheWeek { get; set; }


        [Required]
        public ExitStrategy ExitStrategy { get; set; }
    }
}
