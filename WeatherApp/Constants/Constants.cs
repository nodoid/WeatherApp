using SQLite;

namespace WeatherApp.Constants
{
    public class Constants
    {
        public static string APIKey { get; private set; } = "8701bd54bcc76072ef55857b9c892a3f";
        public static string BaseUri { get; private set; } = "https://api.openweathermap.org/data/2.5/weather?";
        public static SQLiteConnection? DBConnection { get; set; }
    }
}
