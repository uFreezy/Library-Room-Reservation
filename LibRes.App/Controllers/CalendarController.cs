using System;
using System.Collections.Generic;
using System.Linq;
using LibRes.App.Data;
using LibRes.App.DbModels;
using LibRes.App.Models.Calendar;
using LibRes.App.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace LibRes.App.Controllers
{
    [Authorize]
    public class CalendarController : BaseController
    {
        [HttpGet]
        [Route("/event")]
        public IActionResult ViewEvent(int eventId)
        {

            // NOTE: Delete probably
            return View();
        }

        [HttpGet]
        public PartialViewResult ViewEventPartial(int eventId)
        {
            EventSingleView res = Context.ReservationModels
                                         .Include(r => r.EventDates)
                                          .Include(r => r.MeetingRoom)
                                          .Include(r => r.ReservationOwner)
                                         .Where(r => r.Id == eventId)
                                         .Select(r => new EventSingleView{
                EventName = r.EventName,
                InitialDate = r.EventDates.First().Occurance,
                RepeatDates = r.EventDates
                               .Where(d=> d.Id != r.EventDates.First().Id)
                               .Select(d => d.Occurance).ToList(),
                BeginHour = r.BeginHour,
                EndHour = r.EndHour,
                MeetingRoom = r.MeetingRoom.RoomName,
                Department = r.Department,
                ReservationOwner = r.ReservationOwner.Email,
                WantsMultimedia = r.WantsMultimedia
            }).FirstOrDefault();


            return PartialView("_ViewEventPartial", res);
        }

        [HttpGet]
        [Route("/events")]
        public IActionResult ViewEvents()
        {
            // TEMP: Currently used for testing.
            DateTime from = DateTime.MinValue;
            DateTime to = DateTime.MaxValue;
            List<EventOccuranceModel> eventOccurances = Context.EventOccurances
                                                               .Include(o => o.Reservation)
                                                               .Where(o => o.Occurance >= from && o.Occurance <= to)
                                                               .ToList();


            List<EventShortView> parsedEvents = new List<EventShortView>();

            foreach (EventOccuranceModel occ in eventOccurances)
            {
                parsedEvents.Add(new EventShortView
                {
                    Id = occ.Reservation.Id,
                    Name = occ.Reservation.EventName,
                    BeginDate = occ.Occurance.ToString("yyyy-MM-dd") + "T" + occ.Reservation.BeginHour+":00",
                    EndDate = occ.Occurance.ToString("yyyy-MM-dd") + "T" + occ.Reservation.EndHour+":00"
                });
            }

            string eventsJson = JsonConvert.SerializeObject(parsedEvents);

            return View("ViewEvents", eventsJson);

        }


        [HttpGet]
        [Route("/create")]
        public IActionResult CreateEvent()
        {
            // Selects the Available rooms ids and names to display
            // in a dropdown.
            IEnumerable<SelectListItem> rooms = this.Context
                                                    .RoomModels
                                                    .Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text = r.RoomName

            });
            ViewBag.Rooms = rooms;


            CreateEventModel model = new CreateEventModel
            {
                BeginHour = DateTime.Now,
                EndHour = DateTime.Now.AddHours(1),
                EventDate = DateTime.Now
            };

            return View(model);
        }


  
        [HttpPost("/create")]
        [ValidateAntiForgeryToken]
        public IActionResult CreateEvent(CreateEventModel model)
        {
            if(ModelState.IsValid){

                // Does not allow to create event in the past.
                if(model.EventDate < DateTime.Now)
                {
                    ModelState.AddModelError("EventDate", "Date cannot be set to past date!");

                    // Selects the Available rooms ids and names to display
                    // in a dropdown.

                    //Temp
                    ViewBag.Rooms = this.Context.RoomModels
                        .Select(r => new SelectListItem
                        {
                            Value = r.Id.ToString(),
                            Text = r.RoomName

                        });
               
                    return View(model);
                }

                ReservationModel reservation = new ReservationModel
                {
                    EventName = model.EventName,
                    BeginHour = model.BeginHour.Hour.ToString() +':'+ model.BeginHour.Minute.ToString(),
                    EndHour = model.EndHour.Hour.ToString() +':'+ model.EndHour.Minute.ToString(),
                    EventDates = new HashSet<EventOccuranceModel>(),
                    MeetingRoom = this.Context.RoomModels
                                      .FirstOrDefault(r => r.Id.ToString() == model.MeetingRoomId),
                    Department = model.Department,
                    ReservationOwner =  this.Context.Users.FirstOrDefault(u => u.NormalizedUserName == this.User.Identity.Name),
                    WantsMultimedia = model.WantsMultimedia,
                    Description = model.Description
                };

                // Adds initial date.
                reservation.EventDates.Add(new EventOccuranceModel
                {
                    Occurance = model.EventDate,
                    Reservation = reservation
                });

                if (model.IsReoccuring)
                {
                    this.SetDateOccurances(model, reservation);
                }


                Context.AddRange(reservation);
                Context.SaveChanges();

                return RedirectToAction("ViewEvents", "Calendar");
            }

            // Temp
            IEnumerable<SelectListItem> rooms = this.Context
                                                  .RoomModels
                                                  .Select(r => new SelectListItem
                                                  {
                                                      Value = r.Id.ToString(),
                                                      Text = r.RoomName

                                                  });
            ViewBag.Rooms = rooms;
            return View(model);
        }

        public IActionResult EditEvent(int eventId){
            // TODO
            return View();
        }

        [NonAction]
        private List<DayOfWeek> SelectedDays(List<DaysOfWeekEnumModel> daysOfWeekInp)
        {
            List<DayOfWeek> selectedDays = new List<DayOfWeek>();

            foreach (var day in daysOfWeekInp)
            {
                if(day.IsSelected)
                {
                    selectedDays.Add(day.DaysOfWeek);
                }
            }

            selectedDays.Reverse();

            return selectedDays;
        }

        [NonAction]
        private DateTime GetNextMonday(DateTime date)
        {
            int daysUntil = ((int)date.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            DateTime nextOccurance = date.AddDays(daysUntil-1);

            return nextOccurance;
        }
        [NonAction]
        private DateTime GetNextWeekday(DateTime start, DayOfWeek day)
        {
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
            if (daysToAdd == 0) return start.AddDays(7);
            return start.AddDays(daysToAdd);
        }


        [NonAction]
        private void SetDateOccurances(CreateEventModel model, ReservationModel reservation)
        {
            DateTime beginDate = model.EventDate;

            bool isWeekly = model.EventRepeatModel.RepeatOption == EventRepeatOptions.WEEKLY;
            bool isMonthly = model.EventRepeatModel.RepeatOption == EventRepeatOptions.MONTHLY;

            DateTime currentWeek = isWeekly
                ? this.GetNextMonday(model.EventDate)
                                         .AddDays((model.EventRepeatModel.RepeatInterval - 1) * 7)
                : model.EventDate.AddMonths(model.EventRepeatModel.RepeatInterval);

            /*
                If its weekly, get next week's monday and add the repat interval to get next date.
                If its monthly just add one month.
            */

            DateTime lastUpdated = DateTime.MinValue;

            DateTime quitDate = model.EventRepeatModel.ExitStrategy == ExitStrategy.FIXED_DATE ? model.EventRepeatModel.ExitDate : beginDate.AddYears(1);
            while (currentWeek < quitDate)
            {
                List<DayOfWeek> selectedDays = this.SelectedDays(model.EventRepeatModel.DaysOfTheWeek);

                if (isWeekly && currentWeek < lastUpdated.AddDays((model.EventRepeatModel.RepeatInterval * 7) - 7))
                {
                    currentWeek = currentWeek.AddDays(7);
                    continue;
                }
                else if (isMonthly && currentWeek < lastUpdated.AddMonths(1))
                {
                    currentWeek = currentWeek.AddMonths(1);
                    continue;
                }

                foreach (var day in selectedDays)
                {
                    DateTime nextOccurance = GetNextWeekday(currentWeek, day);

                    reservation.EventDates.Add(new EventOccuranceModel
                    {
                        Occurance = nextOccurance,
                        Reservation = reservation
                    });

                    currentWeek = nextOccurance;
                    lastUpdated = nextOccurance;
                }
            }
        }

    }
}