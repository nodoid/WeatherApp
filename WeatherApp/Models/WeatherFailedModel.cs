using Newtonsoft.Json;
using System.Collections.Generic;

namespace WeatherApp.Models
{
    public class WeatherFailedModel
    {
#nullable enable
        [JsonProperty("message")]
        public string? Message { get; set; }
        [JsonProperty("list")]
        public List<object>? List { get; set; }
#nullable disable
        [JsonProperty("cod")]
        public int Cod { get; set; }
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
