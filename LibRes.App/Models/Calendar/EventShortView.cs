using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace LibRes.App.Models.Calendar
{
    public class EventShortView
    {
        [Required] [JsonProperty("id")] public int Id { get; set; }

        [Required] [JsonProperty("title")] public string Name { get; set; }

        [Required] [JsonProperty("start")] public string BeginDate { get; set; }

        [Required] [JsonProperty("end")] public string EndDate { get; set; }
    }
}