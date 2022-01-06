using System.Threading.Tasks;
using WeatherApp.Models;

namespace WeatherApp.Interfaces
{
    public interface IWeatherService
    {
        Task<WeatherData> GetWeatherForLocation(double lng, double lat);
        Task<WeatherData> GetWeatherForCity(string city, string country, string state ="");
    }
}
