using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace LibRes.App.Models.Calendar
{
    public class EventShortView
    {
        [Required]
        [JsonProperty("id")]
        public int Id { get; set; }

        [Required]
        [JsonProperty("title")]
        public String Name { get; set; }

        [Required]
        [JsonProperty("start")]
        public String BeginDate { get; set; }

        [Required]
        [JsonProperty("end")]
        public String EndDate { get; set; }



    }
}
