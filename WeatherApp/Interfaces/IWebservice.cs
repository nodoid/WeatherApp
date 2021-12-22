using System.Threading.Tasks;
using WeatherApp.Models;

namespace WeatherApp.Interfaces
{
    public interface IWebservice
    {
        Task<WeatherData> GetWeatherForLocation(double lng, double lat);
        Task<WeatherData> GetWeatherForCity(string city, string country, string state ="");
    }
}
