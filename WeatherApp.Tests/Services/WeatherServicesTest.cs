using Flurl.Http;
using Flurl.Http.Testing;
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
    [TestFixture]
    public class WeatherServicesTest<T> : IClientApi<T> where T : class
    {
        HttpClient Client;
        IConnection Connection;

        public WeatherServicesTest(HttpMessageHandler httpMessageHandler, IConnection connection)
        {
            Client = SetupClientTest(httpMessageHandler);
            Connection = connection;
            Assert.IsNotNull(Connection);
            Assert.IsNotNull(Client);
        }

        [Test]
        public async Task GetWeatherForCity(string city, string country, string state = "")
        {
            var api = "q=";
            var pars = string.IsNullOrEmpty(state) ? $"{city},{country}" : $"{city},{state},{country}";
            var data = await GetRequestAsync<WeatherData>(api, pars);
            Assert.IsNotNull(data);
            Assert.Equals("Liverpool", data.Name);
        }

        async Task<T1> GetRequestAsync<T1>(string api, string pars)
        {
            return await GetRequestAsync<T1>(api, pars);
        }

        [Test]
        public async Task GetWeatherForLocation(double lng, double lat)
        {
            var api = "lat=";
            var pars = $"{lat}&lon={lng}";
            var data = await GetRequestAsync<WeatherData>(api, pars);
            Assert.IsNotNull(data);
            Assert.Equals("Liverpool", data.Name);
        }

        [Test]
        public HttpClient SetupClientTest(HttpMessageHandler httpMessageHandler)
        {
            Assert.IsNotNull(httpMessageHandler);
            return new HttpClient(httpMessageHandler);
        }


        [Test]
        public void SetupClientTestIsNotNull(HttpMessageHandler httpMessageHandler)
        {
            Assert.IsNotNull(httpMessageHandler);
            Client = new HttpClient(httpMessageHandler)
            {
                BaseAddress = new Uri(Constants.Constants.BaseUri)
            };
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Assert.NotNull(Client.BaseAddress);
            Assert.Equals(1, Client.DefaultRequestHeaders.Accept.Count);
            Assert.NotNull(Client);
        }

        [Test]
        public async Task<T> GetRequestAsync(string apiUrl, string pars)
        {
            var result = Activator.CreateInstance<T>();
            Assert.IsNotNull(result);
            Assert.IsTrue(Connection.NetworkConnected());

            Assert.IsNotNullOrEmpty(pars);
            Assert.IsNotNullOrEmpty(apiUrl);
            Assert.IsNotNullOrEmpty(Constants.Constants.BaseUri);
            Assert.IsNotNullOrEmpty(Constants.Constants.APIKey);

            using (var test = new FlurlClient($"{Constants.Constants.BaseUri}"))
            {
                var resp = await test.WithHeaders($"{apiUrl}{pars}&appid={Constants.Constants.APIKey}").
                    Request(Constants.Constants.BaseUri).
                    AllowAnyHttpStatus().GetJsonAsync();
                Assert.NotNull(resp);
                Assert.Equals(200, resp.StatusCode);
                Assert.IsNotNullOrEmpty(resp.Content);
                var data = await resp.Content.ReadAsStringAsync();
                Assert.NotNull(data);
                result = JsonConvert.DeserializeObject<T>(data);
                Assert.NotNull(result);
            }

            return result;
        }
    }
}
