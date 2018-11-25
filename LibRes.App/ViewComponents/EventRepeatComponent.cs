using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibRes.App.Models.Calendar;
using LibRes.App.Models.Enums;
using Microsoft.AspNetCore.Mvc;

namespace LibRes.App.ViewComponents
{
    public class EventRepeatComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // WARNING: Shitty code 

            CreateEventModel model = new CreateEventModel
            {
                EventRepeatModel = new EventRepeatViewModel
                {
                    RepeatOption = EventRepeatOptions.WEEKLY,
                    DaysOfTheWeek = new List<DaysOfWeekEnumModel>(),
                    ExitDate = DateTime.Now.AddYears(1)
                }
            };


            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                model.EventRepeatModel.DaysOfTheWeek
                   .Add(new DaysOfWeekEnumModel()
                   {
                       DaysOfWeek = day,
                       IsSelected = false
                   });
            }

            return View("~/Views/Calendar/_EventRepeatModal.cshtml", model);
        }
    }
}
