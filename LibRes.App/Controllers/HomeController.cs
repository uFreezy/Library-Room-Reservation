﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using LibRes.App.Models;
using LibRes.App.Models.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibRes.App.Controllers
{
    public class HomeController : BaseController
    {
        /// <summary>
        ///     Gets basic statistics for the front page + The room availability.
        /// </summary>
        /// <returns>Index View</returns>
        public IActionResult Index()
        {
            if (!HttpContext.User.Identity.IsAuthenticated) return RedirectToAction("Login", "Account");

            var todayEvents = Context.EventOccurrences
                .Count(e => e.Reservation.CreatedOn.Date == DateTime.Today);
            var totalEvents = Context.EventOccurrences.Count();
            var eventsFromUser = Context.EventOccurrences
                .Count(e => e.Reservation.ReservationOwner.Id ==
                            HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var occurenceModels = Context.EventOccurrences
                .Include(e => e.Reservation)
                .Include(e => e.Reservation.MeetingRoom)
                .ToList();

            var roomsAvailability = new Dictionary<string, bool>();

            Context.RoomModels.ToList().ForEach(r => { roomsAvailability.Add(r.RoomName, true); });

            foreach (var occ in occurenceModels)
                if (DateTime.Now.Ticks > occ.Occurence.Ticks &&
                    DateTime.Now.Ticks < TimeSpan.FromMinutes(occ.DurationMinutes).Ticks)
                    roomsAvailability[occ.Reservation.MeetingRoom.RoomName] = false;

            return View(new HomeStatisticModel
            {
                EventsCreatedToday = todayEvents,
                TotalEvents = totalEvents,
                TotalEventsCurrentUser = eventsFromUser,
                RoomAvailability = roomsAvailability
            });
        }

        /// <summary>
        ///     About page with basic info about the project.
        /// </summary>
        /// <returns>About View.</returns>
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        /// <summary>
        ///     Gets the 404 error view.
        /// </summary>
        /// <returns>Error View.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
                {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}