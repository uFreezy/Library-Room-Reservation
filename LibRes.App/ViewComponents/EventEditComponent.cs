using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibRes.App.Data;
using LibRes.App.Models.Calendar;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LibRes.App.ViewComponents
{
    public class EventEditComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(int occurrenceId)
        {
            var context = new LibResDbContext();
            var ev = context.EventOccurrences
               .Include(o => o.Reservation)
                .Where( r => r.Id == occurrenceId)
                .Select(o=> new EventEditModel
                {
                    Id = o.Id,
                    EventName = o.Reservation.EventName,
                    MeetingRoomId = o.Reservation.MeetingRoom.Id.ToString(),
                    BeginHour = DateTime.Parse(o.Occurence.ToString("HH:mm")),
                    EndHour = DateTime.Parse((o.Occurence+ TimeSpan.FromMinutes(o.DurationMinutes)).ToString("HH:mm")),
                    Department = o.Reservation.Department,
                    WantsMultimedia = o.Reservation.WantsMultimedia,
                    Description = o.Reservation.Description,

                })
                .FirstOrDefault();
                
            // Selects the Available rooms ids and names to display
            // in a dropdown.
            IEnumerable<SelectListItem> rooms = context
                .RoomModels
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.RoomName
                });
            ViewBag.Rooms = rooms;
            
            return View("~/Views/Calendar/_EventEditModal.cshtml", ev);
        }
    }
}