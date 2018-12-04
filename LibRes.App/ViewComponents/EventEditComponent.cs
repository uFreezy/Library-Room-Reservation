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
        public async Task<IViewComponentResult> InvokeAsync(int eventId)
        {
            var context = new LibResDbContext();
            var ev = context.ReservationModels
                .Include(r => r.ReservationOwner)
                .Include(r => r.MeetingRoom)
                .Include(r => r.EventDates)
                .Where( r => r.Id == eventId)
                .Select(r=> new EventEditModel
                {
                    Id = r.Id,
                    EventName = r.EventName,
                    EventDate = r.EventDates.First().Occurance,
                    MeetingRoomId = r.MeetingRoom.Id.ToString(),
                    //EventRepeatModel = 
                    EventDates = r.EventDates,
                    BeginHour = DateTime.Parse(r.EventDates.First().Occurance.ToString("HH:mm")),
                    EndHour = DateTime.Parse((r.EventDates.First().Occurance- TimeSpan.FromMinutes(r.EventDates.First().DurrationMinutes)).ToString("HH:mm")),
                   // MeetingRoom = r.MeetingRoom,
                    Department = r.Department,
                    //ReservationOwner = r.ReservationOwner,
                    WantsMultimedia = r.WantsMultimedia,
                    Description = r.Description
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