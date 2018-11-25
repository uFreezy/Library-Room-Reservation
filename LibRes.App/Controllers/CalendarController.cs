using System;
using System.Collections.Generic;
using System.Linq;
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
        public PartialViewResult ViewEventPartial(int eventId)
        {
            var res = Context.ReservationModels
                .Include(r => r.EventDates)
                .Include(r => r.MeetingRoom)
                .Include(r => r.ReservationOwner)
                .Where(r => r.Id == eventId)
                .Select(r => new EventSingleView
                {
                    EventName = r.EventName,
                    InitialDate = r.EventDates.First().Occurance,
                    RepeatDates = r.EventDates
                        .Where(d => d.Id != r.EventDates.First().Id)
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
            var from = DateTime.MinValue;
            var to = DateTime.MaxValue;
            var eventOccurrences = Context.EventOccurances
                .Include(o => o.Reservation)
                .Where(o => o.Occurance >= from && o.Occurance <= to)
                .ToList();


            var parsedEvents = new List<EventShortView>();

            foreach (var occ in eventOccurrences)
                parsedEvents.Add(new EventShortView
                {
                    Id = occ.Reservation.Id,
                    Name = occ.Reservation.EventName,
                    BeginDate = occ.Occurance.ToString("yyyy-MM-dd") + "T" + occ.Reservation.BeginHour + ":00",
                    EndDate = occ.Occurance.ToString("yyyy-MM-dd") + "T" + occ.Reservation.EndHour + ":00"
                });

            var eventsJson = JsonConvert.SerializeObject(parsedEvents);

            return View("ViewEvents", eventsJson);
        }


        [HttpGet]
        [Route("/create")]
        public IActionResult CreateEvent()
        {
            // Selects the Available rooms ids and names to display
            // in a dropdown.
            IEnumerable<SelectListItem> rooms = Context
                .RoomModels
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.RoomName
                });
            ViewBag.Rooms = rooms;


            var model = new CreateEventModel
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
            if (ModelState.IsValid)
            {
                // Does not allow to create event in the past.
                if (model.EventDate < DateTime.Now)
                {
                    ModelState.AddModelError("EventDate", "Date cannot be set to past date!");

                    // Selects the Available rooms ids and names to display
                    // in a dropdown.

                    //Temp
                    ViewBag.Rooms = Context.RoomModels
                        .Select(r => new SelectListItem
                        {
                            Value = r.Id.ToString(),
                            Text = r.RoomName
                        });

                    return View(model);
                }

                var reservation = new ReservationModel
                {
                    EventName = model.EventName,
                    BeginHour = model.BeginHour.Hour.ToString() + ':' + model.BeginHour.Minute,
                    EndHour = model.EndHour.Hour.ToString() + ':' + model.EndHour.Minute,
                    EventDates = new HashSet<EventOccuranceModel>(),
                    MeetingRoom = Context.RoomModels
                        .FirstOrDefault(r => r.Id.ToString() == model.MeetingRoomId),
                    Department = model.Department,
                    ReservationOwner = Context.Users.FirstOrDefault(u => u.NormalizedUserName == User.Identity.Name),
                    WantsMultimedia = model.WantsMultimedia,
                    Description = model.Description
                };

                // Adds initial date.
                reservation.EventDates.Add(new EventOccuranceModel
                {
                    Occurance = model.EventDate,
                    Reservation = reservation
                });

                if (model.IsReoccuring) SetDateOccurances(model, reservation);


                Context.AddRange(reservation);
                Context.SaveChanges();

                return RedirectToAction("ViewEvents", "Calendar");
            }

            // Temp
            IEnumerable<SelectListItem> rooms = Context
                .RoomModels
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.RoomName
                });
            ViewBag.Rooms = rooms;
            return View(model);
        }

        public IActionResult EditEvent(int eventId)
        {
            // TODO
            return View();
        }

        [NonAction]
        private static IEnumerable<DayOfWeek> SelectedDays(IEnumerable<DaysOfWeekEnumModel> daysOfWeekInp)
        {
            var selectedDays = (from day in daysOfWeekInp where day.IsSelected select day.DaysOfWeek).ToList();

            selectedDays.Reverse();

            return selectedDays;
        }

        [NonAction]
        private static DateTime GetNextMonday(DateTime date)
        {
            var daysUntil = ((int) date.DayOfWeek - (int) DayOfWeek.Monday + 7) % 7;
            var nextOccurence = date.AddDays(daysUntil - 1);

            return nextOccurence;
        }

        [NonAction]
        private static DateTime GetNextWeekday(DateTime start, DayOfWeek day)
        {
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            var daysToAdd = ((int) day - (int) start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd == 0 ? 7 : daysToAdd);
        }


        [NonAction]
        private static void SetDateOccurances(CreateEventModel model, ReservationModel reservation)
        {
            var beginDate = model.EventDate;

            var isWeekly = model.EventRepeatModel.RepeatOption == EventRepeatOptions.Weekly;
            var isMonthly = model.EventRepeatModel.RepeatOption == EventRepeatOptions.Monthly;

            var currentWeek = isWeekly
                ? GetNextMonday(model.EventDate)
                    .AddDays((model.EventRepeatModel.RepeatInterval - 1) * 7)
                : model.EventDate.AddMonths(model.EventRepeatModel.RepeatInterval);

            /*
                If its weekly, get next week's monday and add the repeat interval to get next date.
                If its monthly just add one month.
            */

            var lastUpdated = DateTime.MinValue;

            var quitDate = model.EventRepeatModel.ExitStrategy == ExitStrategy.FIXED_DATE
                ? model.EventRepeatModel.ExitDate
                : beginDate.AddYears(1);
            while (currentWeek < quitDate)
            {
                var selectedDays = SelectedDays(model.EventRepeatModel.DaysOfTheWeek);

                if (isWeekly && currentWeek < lastUpdated.AddDays(model.EventRepeatModel.RepeatInterval * 7 - 7))
                {
                    currentWeek = currentWeek.AddDays(7);
                    continue;
                }

                if (isMonthly && currentWeek < lastUpdated.AddMonths(1))
                {
                    currentWeek = currentWeek.AddMonths(1);
                    continue;
                }

                foreach (var day in selectedDays)
                {
                    var nextOccurence = GetNextWeekday(currentWeek, day);

                    reservation.EventDates.Add(new EventOccuranceModel
                    {
                        Occurance = nextOccurence,
                        Reservation = reservation
                    });

                    currentWeek = nextOccurence;
                    lastUpdated = nextOccurence;
                }
            }
        }
    }
}