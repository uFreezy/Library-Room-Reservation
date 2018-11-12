using System;
using System.Collections.Generic;
using System.Linq;
using LibRes.App.Data;
using LibRes.App.DbModels;
using LibRes.App.Models;
using LibRes.App.Models.Calendar;
using LibRes.App.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibRes.App.Controllers
{
    [Authorize]
    public class CalendarController : BaseController
    {
        [HttpGet]
        [Route("/event")]
        public IActionResult ViewEvent(int eventId)
        {

            // NOTE: Subject to change, most likely the view will be partial.
            return View();
        }

        [HttpGet]
        [Route("/events")]
        public IActionResult ViewEvents(DateTime from, DateTime to)
        {
            return View();
        }


        [HttpGet]
        [Route("/create")]
        public IActionResult CreateEvent()
        {
            // Selects the Available rooms ids and names to display
            // in a dropdown.
            var libResDbContext = new LibResDbContext();
            IEnumerable<SelectListItem> rooms = this.Context
                                                    .RoomModels
                                                    .Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text = r.RoomName

            });
            ViewBag.Rooms = rooms;

            // WARNING: Shitty code 
            CreateEventModel cem = new CreateEventModel
            {
                EventRepeatModel = new EventRepeatViewModel()
            };
            cem.EventRepeatModel.DaysOfTheWeek = new List<DaysOfWeekEnumModel>();

            foreach (DaysOfTheWeek day in Enum.GetValues(typeof(DaysOfTheWeek)))
            {
                cem.EventRepeatModel.DaysOfTheWeek
                   .Add(new DaysOfWeekEnumModel() { 
                    DaysOfWeek = day, 
                    IsSelected = false 
                });
            }

            return View(cem);
        }


        [HttpPost("/create")]
        [ValidateAntiForgeryToken]
        public IActionResult CreateEvent(CreateEventModel model)
        {
            if(ModelState.IsValid){

                ReservationModel reservation = new ReservationModel
                {
                    EventName = model.EventName,
                    BeginHour = model.BeginHour.ToString(),
                    EndHour = model.EndHour.ToString(),
                    EventDates = new HashSet<EventOccuranceModel>(),
                    MeetingRoom = this.Context.RoomModels
                                      .FirstOrDefault(r => r.Id.ToString() == model.MeetingRoomId),
                    Department = model.Department,
                    ReservationOwner =  this.Context.Users.FirstOrDefault(u => u.NormalizedUserName == this.User.Identity.Name),
                    WantsMultimedia = model.WantsMultimedia,
                    Description = model.Description
                };

              /*  if(model.IsReoccuring)
                {
                    for (int i = 0; i < model.EventRepeatModel.RepeatInterval; i++)
                    {
                        if (model.EventRepeatModel.RepeatOption == EventRepeatOptions.WEEKLY){
                            //reservation.EventDates.Add(new DateTime())
                            reservation.EventDates.Add(new EventOccuranceModel()
                            {
                                Occurance = new DateTime(model.EventDate.Day),
                                Reservation = reservation
                            });
                        } else 
                        {

                        }
                    }
                }*/

              

                // TODO: Handle multiple ocurances 
                reservation.EventDates.Add(new EventOccuranceModel{
                    Occurance = model.EventDate,
                    Reservation = reservation
                });

                Context.AddRange(reservation);
                Context.SaveChanges();

                //TODO: Create view 
                return RedirectToAction("ViewEvents", "Calendar");
            }

            return View();
        }

        public IActionResult EditEvent(){
            // Maybe we can use the same form as create ?
            return View();
        }

    }
}
