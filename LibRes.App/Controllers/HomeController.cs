using System;
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

            foreach (var occ in occurenceModels)
            {
                if (!roomsAvailability.ContainsKey(occ.Reservation.MeetingRoom.RoomName))
                    roomsAvailability.Add(occ.Reservation.MeetingRoom.RoomName, true);
                if (DateTime.Now.Ticks > occ.Occurence.Ticks &&
                    DateTime.Now.Ticks < TimeSpan.FromMinutes(occ.DurationMinutes).Ticks)
                    roomsAvailability[occ.Reservation.MeetingRoom.RoomName] = false;
            }

            return View(new HomeStatisticModel
            {
                EventsCreatedToday = todayEvents,
                TotalEvents = totalEvents,
                TotalEventsCurrentUser = eventsFromUser,
                RoomAvailability = roomsAvailability
            });
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
                {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}