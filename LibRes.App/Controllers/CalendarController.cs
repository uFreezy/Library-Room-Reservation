using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
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
        /// <summary>
        ///     Renders partial view as modal with info for specific reservation.
        /// </summary>
        /// <param name="eventId">Reservation's id.</param>
        /// <returns>Partial view as modal.</returns>
        [HttpGet]
        public PartialViewResult ViewEventPartial(int eventId)
        {
            var res = Context.EventOccurrences
                .Include(o => o.Reservation)
                .Where(o => o.Id == eventId)
                .Select(o => new EventSingleView
                {
                    Id = o.Id,
                    EventName = o.Reservation.EventName,
                    InitialDate = o.Occurence,
                    RepeatDates = o.Reservation.EventDates
                        .Where(d => d.Id != o.Id)
                        .Select(d => d.Occurence).ToList(),
                    BeginHour = o.Occurence.ToString("HH:mm"),
                    EndHour = (o.Occurence +
                               TimeSpan.FromMinutes(o.DurationMinutes)).ToString("HH:mm"),
                    MeetingRoom = o.Reservation.MeetingRoom.RoomName,
                    Department = o.Reservation.Department,
                    ReservationOwner = o.Reservation.ReservationOwner,
                    WantsMultimedia = o.Reservation.WantsMultimedia,
                    IsOwner = o.Reservation.ReservationOwner.Id == HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value
                }).FirstOrDefault();
            

            return PartialView("_ViewEventPartial", res);
        }


        /// <summary>
        ///     Lists events between two given dates.
        ///     If the request is AJAX
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns>Returns 'ViewEvents' view, if the request is ajax, returns JSON.</returns>
        [HttpGet]
        [Route("/events")]
        public IActionResult ViewEvents(int roomId = -1)
        {
            // TEMP: Currently used for testing.
            var from = DateTime.MinValue;
            var to = DateTime.MaxValue;

            // Sets default room id, which is the first room in the table.
            if (roomId == -1) roomId = Context.RoomModels.First().Id;

            var eventOccurrences = Context.EventOccurrences
                .Include(o => o.Reservation)
                .Where(o => o.Reservation.MeetingRoom.Id == roomId &&
                            o.Occurence >= from && o.Occurence <= to)
                .ToList();


            var parsedEvents = new List<EventShortView>();

            foreach (var occ in eventOccurrences)
                parsedEvents.Add(new EventShortView
                {
                    Id = occ.Id,
                    Name = occ.Reservation.EventName,
                    BeginDate = occ.Occurence.ToString(CultureInfo.InvariantCulture),
                    EndDate =
                        (occ.Occurence + TimeSpan.FromMinutes(occ.DurationMinutes)).ToString(CultureInfo
                            .InvariantCulture)
                });

            SetRoomsToViewBag();

            var eventsJson = JsonConvert.SerializeObject(parsedEvents);

            var isAjaxCall = Request.Headers["x-requested-with"] == "XMLHttpRequest";

            if (isAjaxCall) return new JsonResult(eventsJson);

            return View("ViewEvents", eventsJson);
        }


        /// <summary>
        ///     Serves the view for reservation creation.
        /// </summary>
        /// <returns>View with the form for reservation creation.</returns>
        [HttpGet]
        [Route("/create")]
        public IActionResult CreateEvent()
        {
            SetRoomsToViewBag();

            var model = new CreateEventModel
            {
                BeginHour = DateTime.Now,
                EndHour = DateTime.Now.AddHours(1),
                EventDate = DateTime.Now
            };

            return View(model);
        }


        /// <summary>
        ///     Creates new reservation.
        /// </summary>
        /// <param name="model">Model for creating new reservation.</param>
        /// <returns>Returns same view or 'ViewEvents'.</returns>
        [HttpPost("/create")]
        [ValidateAntiForgeryToken]
        public IActionResult CreateEvent(CreateEventModel model)
        {
            if (ModelState.IsValid)
            {
                // Does not allow to create event in the past.
                // Gives few minutes buffer time.
                var evDate = model.EventDate.AddHours(model.BeginHour.Hour).AddMinutes(model.BeginHour.Minute);
                if (evDate < DateTime.Now - TimeSpan.FromMinutes(3))
                {
                    ModelState.AddModelError("EventDate", "Date cannot be set to past date!");

                    SetRoomsToViewBag();

                    return View(model);
                }

                if (model.BeginHour > model.EndHour)
                {
                    ModelState.AddModelError("EndHour", "The end hour can't be smaller than the beginning.");

                    SetRoomsToViewBag();

                    return View(model);
                }

                var reservation = new ReservationModel
                {
                    EventName = model.EventName,
                    EventDates = new HashSet<EventOccurenceModel>(),
                    MeetingRoom = Context.RoomModels
                        .FirstOrDefault(r => r.Id.ToString() == model.MeetingRoomId),
                    Department = model.Department,
                    ReservationOwner = Context.Users.FirstOrDefault(u => u.NormalizedUserName == User.Identity.Name),
                    WantsMultimedia = model.WantsMultimedia,
                    Description = model.Description,
                    CreatedOn = DateTime.Now
                };

                // Adds initial date.
                reservation.EventDates.Add(new EventOccurenceModel
                {
                    Occurence = model.EventDate.AddHours(model.BeginHour.Hour).AddMinutes(model.BeginHour.Minute),
                    Reservation = reservation,
                    DurationMinutes = (model.EndHour - model.BeginHour).TotalMinutes
                });


                if (BusyDates(reservation).Count != 0)
                {
                    ModelState.AddModelError("EventDate",
                        "Another reservation is already in place in this meeting room for this time frame");

                    SetRoomsToViewBag();

                    return View(model);
                }

                if (model.IsReoccuring)
                {
                    SetDateOccurrences(model, reservation);

                    var busyDates = BusyDates(reservation);

                    if (busyDates.Count != 0)
                    {
                        var errorMsg =
                            "Another reservation is already in place in this meeting room for the following dates: ";

                        foreach (var occ in busyDates) errorMsg += occ.Occurence.ToString(CultureInfo.InvariantCulture);

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

        /// <summary>
        ///     Edits existing reservation event created by the user.
        /// </summary>
        /// <param name="model">
        ///     Model containing the new data to be applied
        ///     to the existing reservation.
        /// </param>
        /// <returns>Returns 'View Events' or the event edit modal form.</returns>
        public IActionResult EditEvent(EventEditModel model)
        {
            if (model == null || !ModelState.IsValid) return View("_EventEditModal");
            if (Context.EventOccurrences.First(e => e.Id == model.Id).Occurence < DateTime.Now)
            {
                ModelState.AddModelError("BeginHour", "Cant edit past event");
            }
            
            var ev = Context.ReservationModels
                .Include(r => r.EventDates)
                .Include(r => r.ReservationOwner)
                .First(r => r.Id == Context.EventOccurrences
                                .Include(e => e.Reservation)
                                .First(e => e.Id == model.Id).Reservation.Id);
            
            if (ev.EventDates.First().Occurence == DateTime.Now && model.BeginHour.TimeOfDay < DateTime.Now.TimeOfDay)
            {
                ModelState.AddModelError("BeginHour","Begin date can't be in the past.");
            }

            if (model.EndHour <= model.BeginHour)
            {
                ModelState.AddModelError("EndHour", "End hour can't be less or equals to Begin Hour.");
            }
            
            if(ModelState.ErrorCount > 0) return ViewComponent("EventEditComponent", model);
            
            
            // If the user is not the owner of the event,redirect to home page.
            if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value != ev.ReservationOwner.Id)
                return RedirectToAction("Index", "Home");
            
            ev.EventName = model.EventName;
            ev.MeetingRoom = Context.RoomModels.First(e => e.Id == int.Parse(model.MeetingRoomId));
            ev.Department = model.Department;
            ev.WantsMultimedia = model.WantsMultimedia;
            ev.Description = model.Description;
            
            
            if (model.ShouldApplyForAllDates)
            {
                foreach (var occ in ev.EventDates)
                {
                    if(occ.Occurence.Date < DateTime.Today) continue;
                    occ.Occurence = occ.Occurence.Date.Add(model.BeginHour.TimeOfDay);
                    occ.DurationMinutes = (model.EndHour - model.BeginHour).TotalMinutes;
                }
                    
                var busyDates = BusyDates(ev);

                if (busyDates.Count(b => ev.EventDates.All(e => e.Id != b.Id))!= 0)
                {
                    var errorMsg =
                        "Another reservation is already in place in this meeting room for the following dates: ";

                    foreach (var occ in busyDates) errorMsg += occ.Occurence.ToString(CultureInfo.InvariantCulture);

                    ModelState.AddModelError("BeginHour", errorMsg);

                    SetRoomsToViewBag();

                    return ViewComponent("EventEditComponent", model);
                }
            }
            else
            {
                var occ = ev.EventDates.First(e => e.Id == model.Id);
                occ.Occurence = occ.Occurence.Date.Add(model.BeginHour.TimeOfDay);
                occ.DurationMinutes = (model.EndHour - model.BeginHour).TotalMinutes;

                var mockRes = new ReservationModel
                {
                    EventDates = new List<EventOccurenceModel>(new[] {occ}), MeetingRoom = ev.MeetingRoom
                };

                if (BusyDates(mockRes).Count > 0)
                {
                    const string errorMsg = "Another reservation is already in place in this meeting room for the date";
                    
                    ModelState.AddModelError("BeginHour", errorMsg);

                    SetRoomsToViewBag();
                    
                    // TODO: Fix (Does not display properly)
                    return ViewComponent("EventEditComponent", model);
                    
                }
            }

            Context.ReservationModels.Update(ev);
            Context.SaveChanges();

            return RedirectToAction("ViewEvents");
        }

        /// <summary>
        ///     Executes delete request on a single reservation.
        /// </summary>
        /// <param name="eventId">The event's Id.</param>
        /// <returns>View of all the calendar events.</returns>
        [HttpDelete("remove")]
        public IActionResult DeleteEvent(int eventId)
        {
            if (!Context.ReservationModels.Any(r => r.Id == eventId)) return RedirectToAction("ViewEvents", "Calendar");

            var ev = Context.ReservationModels.Include(r => r.ReservationOwner)
                .First(r => r.Id == eventId);

            if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value != ev.ReservationOwner.Id)
                return RedirectToAction("Index", "Home");

            Context.ReservationModels.Remove(ev);
            Context.SaveChanges();

            return RedirectToAction("ViewEvents", "Calendar");
        }

        /// <summary>
        ///     Gets meeting rooms from the DB and sets them to the ViewBag
        ///     to be used from different views.
        /// </summary>
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
                }).OrderBy(r => r.Value);
            ViewBag.Rooms = rooms;
        }

        /// <summary>
        ///     Returns list of the selected days in DayOfWeekEnumModel.
        /// </summary>
        /// <param name="daysOfWeekInp">List of days to get the selected days from.</param>
        /// <returns>List of objects containing the selected days.</returns>
        [NonAction]
        private static IEnumerable<DayOfWeek> SelectedDays(IEnumerable<DaysOfWeekEnumModel> daysOfWeekInp)
        {
            var selectedDays = (from day in daysOfWeekInp
                where day.IsSelected
                select day.DaysOfWeek).ToList();

            selectedDays.Reverse();

            return selectedDays;
        }

        /// <summary>
        ///     Gets the first occurence of given day of the week from next week.
        /// </summary>
        /// <param name="start">The date to get next given day from.</param>
        /// <param name="day">Day of the week to fetch.</param>
        /// <returns>Datetime of the next occurence.</returns>
        [NonAction]
        private static DateTime GetNextDayOccurence(DateTime start, DayOfWeek day)
        {
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            var daysToAdd = ((int) day - (int) start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd == 0 ? 7 : daysToAdd);
        }

        /// <summary>
        ///     Takes CreateEventModel and bind date occurrences to it
        ///     according to the RepeatStrategy selected by the user.
        /// </summary>
        /// <param name="model"> Model to get the strategies from.</param>
        /// <param name="reservation">Reservation model to bind dates to.</param>
        [NonAction]
        private static void SetDateOccurrences(CreateEventModel model, ReservationModel reservation)
        {
            var beginDate = model.EventDate;

            var isWeekly = model.EventRepeatModel.RepeatOption == EventRepeatOptions.Weekly;
            var isMonthly = model.EventRepeatModel.RepeatOption == EventRepeatOptions.Monthly;

            var currentWeek = isWeekly
                ? GetNextDayOccurence(model.EventDate, DayOfWeek.Monday)
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
                    var nextOccurence = GetNextDayOccurence(currentWeek, day);

                    reservation.EventDates.Add(new EventOccurenceModel
                    {
                        Occurence = nextOccurence.AddHours(model.BeginHour.Hour).AddMinutes(model.BeginHour.Minute),
                        Reservation = reservation,
                        DurationMinutes = (model.EndHour - model.BeginHour).TotalMinutes
                    });

                    currentWeek = nextOccurence;
                    lastUpdated = nextOccurence;
                }
            }
        }

        /// <summary>
        ///     Checks if the selected dates in the ReservationModel is free for reservations
        ///     (No reservation for the same timespan and in the same room in the DB).
        /// </summary>
        /// <param name="model">The model to be checked against the Database.</param>
        /// <returns>Returns list of DateTimes representing the taken dates.</returns>
        [NonAction]
        private List<EventOccurenceModel> BusyDates(ReservationModel model)
        {
            return (from date in model.EventDates
                let begin = date.Occurence
                let end = date.Occurence.AddMinutes(date.DurationMinutes)
                where Context.EventOccurrences.Any(e =>
                    e.Reservation.MeetingRoom.Id.ToString() == model.MeetingRoom.Id.ToString() &&
                    e.Occurence < end &&
                    e.Occurence + TimeSpan.FromMinutes(e.DurationMinutes) > begin)
                select date).ToList();
        }
    }
}