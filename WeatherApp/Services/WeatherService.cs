using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WeatherApp.Helpers;
using WeatherApp.Interfaces;
using WeatherApp.Models;
using WeatherApp.ViewModels;

namespace WeatherApp.Services
{
    public class WeatherService : IWeatherService
    {
        BaseViewModel viewModel => (BaseViewModel)Startup.ServiceProvider.GetService(typeof(BaseViewModel));

        async Task<T> GetSingleItem<T>(string api, string pars)
        {
            var result = Activator.CreateInstance<T>();
            try
            {
                using (var client = new HttpClientHelper().SetupClient(timeOut: 1, isMinutes: true, acceptMediaType: true, mediaType:"application/json"))
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, $"{Constants.Constants.BaseUri}{api}{pars}&appid={Constants.Constants.APIKey}");
                    var send = await client.SendAsync(request);

                    if (send.StatusCode == HttpStatusCode.OK)
                    {
                        var res = await send.Content.ReadAsStringAsync();
                        result = JsonConvert.DeserializeObject<T>(res);
                    }
                }
            }
            catch (Exception ex)
            {
                #if DEBUG
                    Console.WriteLine($"Exception : {ex.Message} : {ex.InnerException?.Message}");
                #endif
            }
            return result;
        }

        public async Task<WeatherData> GetWeatherForCity(string city, string country, string state = "")
        {
            if (viewModel.IsConnected)
            {
                var api = "q=";
                var pars = string.IsNullOrEmpty(state) ? $"{city},{country}" : $"{city},{state},{country}";
                var data = await GetSingleItem<WeatherData>(api, pars);
                return data;
            }
            else
                return default;
        }

        public async Task<WeatherData> GetWeatherForLocation(double lng, double lat)
        {
            if (viewModel.IsConnected)
            {
                var api = "lat=";
                var pars = $"{lat}&lon={lng}";
                var data = await GetSingleItem<WeatherData>(api, pars);
                return data;
            }
            else
                return default;
        }
    }
}
