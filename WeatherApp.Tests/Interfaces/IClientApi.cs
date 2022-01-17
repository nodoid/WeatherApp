using System.Threading.Tasks;

namespace WeatherApp.Tests.Interfaces
{
    public interface IClientApi<T>
    {
        Task<T> TestGetRequestAsync(string apiUrl, string pars = "");
    }
}
