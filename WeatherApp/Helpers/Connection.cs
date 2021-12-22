using WeatherApp.Interfaces;

namespace WeatherApp.Helpers
{
    public class Connection : ISqLiteConnectionFactory
    {
        public virtual void GetConnection()
        {
        }
    }
}
