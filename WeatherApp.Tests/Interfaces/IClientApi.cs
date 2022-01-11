using System.Threading.Tasks;

namespace WeatherApp.Tests.Interfaces
{
    public interface IClientApi<T>
    {
        Task<T> GetRequestAsync(string apiUrl, string pars = "");
    }
}
