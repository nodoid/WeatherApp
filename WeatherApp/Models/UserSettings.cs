using SQLite;

namespace WeatherApp.Models
{
    public class UserSettings
    {
        [PrimaryKey]
        public int Id { get; set; }
        public bool CanUseGeolocation { get; set; }
        public bool IsUSState { get; set; }
        public bool AutoOnStart { get; set; }
#nullable enable
        public string? CityName { get; set; }
        public string? Country { get; set; }
        public string? USState { get; set; }
#nullable disable
        public double LastLat { get; set; }
        public double LastLng { get; set; }
    }
}
