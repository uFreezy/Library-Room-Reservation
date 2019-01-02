using System;
using System.Collections.Generic;

namespace LibRes.App.Models.Home
{
    public class HomeStatisticModel
    {
        public int EventsCreatedToday { get; set; }
        
        public int TotalEvents { get; set; }

        public int TotalEventsCurrentUser { get; set; }
    }
}