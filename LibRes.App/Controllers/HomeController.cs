using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using LibRes.App.Models;
using LibRes.App.Models.Home;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;

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
            
           // TODO: Add if room is busy or not
            return View(new HomeStatisticModel
            {
                EventsCreatedToday = todayEvents,
                TotalEvents = totalEvents,
                TotalEventsCurrentUser = eventsFromUser
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