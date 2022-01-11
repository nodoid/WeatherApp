using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WeatherApp.Models;
using WeatherApp.Tests.Interfaces;

namespace WeatherApp.Tests.Services
{
    public class HttpClientApi<T> : IClientApi<T> where T:class
    {
        HttpClient Client;
        IConnection Connection;

        public HttpClientApi(HttpMessageHandler httpMessageHandler, IConnection connection)
        {
            Client = SetupClient(httpMessageHandler);
            Connection = connection;
        }

        [Test]
        public async Task<WeatherData> GetWeatherForCity(string city, string country, string state = "")
        {
                var api = "q=";
                var pars = string.IsNullOrEmpty(state) ? $"{city},{country}" : $"{city},{state},{country}";
                var data = await GetRequestAsync<WeatherData>(api, pars);
                return data;
        }

        async Task<T1> GetRequestAsync<T1>(string api, string pars)
        {
           return await GetRequestAsync<T1>(api, pars);
        }

        [Test]
        public async Task<WeatherData> GetWeatherForLocation(double lng, double lat)
        {
                var api = "lat=";
                var pars = $"{lat}&lon={lng}";
                var data = await GetRequestAsync<WeatherData>(api, pars);
                return data;
        }

        /// <summary>
        /// Setups the client.
        /// </summary>
        /// <param name="httpMessageHandler">The HTTP message handler.</param>
        /// <returns>a httpClient</returns>
        HttpClient SetupClient(HttpMessageHandler httpMessageHandler)
        {
            Client = new HttpClient(httpMessageHandler)
            {
                BaseAddress = new Uri(Constants.Constants.BaseUri)
            };
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return Client;
        }

        public async Task<T> GetRequestAsync(string apiUrl, string pars)
        {
            var result = Activator.CreateInstance<T>();
            if (Connection.NetworkConnected())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{Constants.Constants.BaseUri}{apiUrl}{pars}&appid={Constants.Constants.APIKey}");
                var response = await Client.SendAsync(request).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<T>(responseString);
            }

            return result;
        }
    }
}
