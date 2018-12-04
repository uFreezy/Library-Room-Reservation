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
                    Id = r.Id,
                    EventName = r.EventName,
                    InitialDate = r.EventDates.First().Occurance,
                    RepeatDates = r.EventDates
                        .Where(d => d.Id != r.EventDates.First().Id)
                        .Select(d => d.Occurance).ToList(),
                    BeginHour = r.EventDates.First().Occurance.ToString("HH:mm"),
                    EndHour = (r.EventDates.First().Occurance-TimeSpan.FromMinutes(r.EventDates.First().DurrationMinutes)).ToString("HH:mm"),
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
                    // TODO: Update to get the time from Occurance using the duration
                    BeginDate = occ.Occurance.ToString(),//.ToString("yyyy-MM-dd") + "T" + occ.Reservation.BeginHour + ":00",
                    EndDate = (occ.Occurance+TimeSpan.FromMinutes(occ.DurrationMinutes)).ToString()//.ToString("yyyy-MM-dd") + "T" + occ.Reservation.EndHour + ":00"
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


        public IActionResult IsAvailable(DateTime begin, DateTime end, string meetingRoomId)
        {
            if (Context.RoomModels.First(r => r.Id.ToString() == meetingRoomId).RoomReservations.Any(rr =>
                rr.EventDates.First().Occurance >= begin && rr.EventDates.First().Occurance <= end))
                return Conflict("Already exists!");
            return Ok("Available!");
        }

        [HttpPost("/create")]
        [ValidateAntiForgeryToken]
        public IActionResult CreateEvent(CreateEventModel model)
        {
            if (ModelState.IsValid)
            {
                // Does not allow to create event in the past.
                // Gives few mins buffer time.
                // TODO: Check if endhour !< beginhour
                var evDate = model.EventDate.AddHours(model.BeginHour.Hour).AddMinutes(model.BeginHour.Minute);
                if (evDate < DateTime.Now - TimeSpan.FromMinutes(3))
                {
                    ModelState.AddModelError("EventDate", "Date cannot be set to past date!");

                    SetRoomsToViewBag();

                    return View(model);
                }

                var reservation = new ReservationModel
                {
                    EventName = model.EventName,
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
                    Occurance = model.EventDate.AddHours(model.BeginHour.Hour).AddMinutes(model.BeginHour.Minute),
                    Reservation = reservation,
                    DurrationMinutes = (model.EndHour - model.BeginHour).TotalMinutes
                });


                if (BusyDates(reservation, model).Count != 0)
                {
                    ModelState.AddModelError("EventDate",
                        "Another reservation is already in place in this meeting room for this timeframe");

                    SetRoomsToViewBag();

                    return View(model);
                }

                if (model.IsReoccuring)
                {
                    SetDateOccurances(model, reservation);

                    var busyDates = BusyDates(reservation, model);

                    if (busyDates.Count != 0)
                    {
                        var errorMsg =
                            "Another reservation is already in place in this meeting room for the following dates: ";

                        foreach (var date in busyDates) errorMsg += date.ToString();

                        ModelState.AddModelError("EventDate", errorMsg);

                        SetRoomsToViewBag();

                        return View(model);
                    }
                }

                Context.AddRange(reservation);
                Context.SaveChanges();

                return RedirectToAction("ViewEvents", "Calendar");
            }

            SetRoomsToViewBag();

            return View(model);
        }


        public IActionResult EditEvent(EventEditModel model, int eventId)
        {
            if (model == null || !ModelState.IsValid) return View("_EventEditModal");
            var ev = Context.ReservationModels.First(r => r.Id == eventId);

            if (model.EventName != null) ev.EventName = model.EventName;
            if (model.EventDates.First().Occurance.AddHours(model.BeginHour.Hour)
                    .AddMinutes(model.BeginHour.Minute) < DateTime.Now ||
                model.EventDates.First().Occurance.AddHours(model.EndHour.Hour)
                    .AddMinutes(model.EndHour.Minute) < DateTime.Now
            )
            {
                ModelState.AddModelError("EventDates", "Dates cannot be in the past");

                return View("_EventEditModal", model);
            }

            /*ev.BeginHour = model.BeginHour.ToString();
            ev.EndHour = model.EndHour.ToString();*/
            var date = ev.EventDates.First().Occurance;
            date = model.EventDate;

            // TODO: Implement the rest of the update. Left for discussion.


            Context.ReservationModels.Update(ev);
            Context.SaveChanges();

            return View("ViewEvents");
        }

        public IActionResult DeleteEvent()
        {
            // TODO
            return View();
        }

        [NonAction]
        private void SetRoomsToViewBag()
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


            var lastUpdated = DateTime.MinValue;

            var quitDate = model.EventRepeatModel.ExitStrategy == ExitStrategy.FIXED_DATE
                ? model.EventRepeatModel.ExitDate
                : beginDate.AddYears(1);
            while (currentWeek < quitDate)
            {
                var selectedDays = SelectedDays(model.EventRepeatModel.DaysOfTheWeek);

                /*
                    If its weekly, get next week's monday and add the repeat interval to get next date.
                    If its monthly just add one month.
                */
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
                        Occurance = nextOccurence.AddHours(model.BeginHour.Hour).AddMinutes(model.BeginHour.Minute),
                        Reservation = reservation,
                        DurrationMinutes = (model.EndHour - model.BeginHour).TotalMinutes
                    });

                    currentWeek = nextOccurence;
                    lastUpdated = nextOccurence;
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="model"></param>
        /// <param name="crModel"></param>
        /// <returns></returns>
        [NonAction]
        private List<DateTime> BusyDates(ReservationModel model, CreateEventModel crModel)
        {
            return (from date in model.EventDates
                let begin = date.Occurance
                let end = date.Occurance.AddMinutes(date.DurrationMinutes)
                where Context.EventOccurances.Any(e =>
                    e.Reservation.MeetingRoom.Id.ToString() == model.MeetingRoom.Id.ToString() &&
                    e.Occurance < end &&
                    e.Occurance + TimeSpan.FromMinutes(e.DurrationMinutes) > begin)
                select date.Occurance).ToList();
        }
    }
}