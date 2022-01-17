using System.Threading.Tasks;

namespace WeatherApp.Tests.Interfaces
{
    public interface IClientApi<T>
    {
        Task<T> Test_GetRequestAsync(string apiUrl, string pars = "");
    }
}
