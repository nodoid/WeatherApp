using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using WeatherApp.Models;
using WeatherApp.Tests.Interfaces;
using WeatherApp.Tests.Models;

namespace WeatherApp.Tests.Services
{
    [TestFixture]
    public class WeatherServiceTest<T> : IClientApi<T> where T : class
    {
        IConnection Connection = Substitute.For<IConnection>();

        [Test]
        public async Task TestGetWeatherForCity(string city, string country, string state = "")
        {
            var api = "q=";
            var pars = string.IsNullOrEmpty(state) ? $"{city},{country}" : $"{city},{state},{country}";
            var data = await TestGetRequestAsync<WeatherData>(api, pars);
            Assert.IsNotNull(data);
            Assert.Equals("Liverpool", data.Name);
        }

        async Task<T1> TestGetRequestAsync<T1>(string api, string pars)
        {
            return await TestGetRequestAsync<T1>(api, pars);
        }

        [Test]
        public async Task TestGetWeatherForLocation(double lng, double lat)
        {
            var api = "lat=";
            var pars = $"{lat}&lon={lng}";
            var data = await TestGetRequestAsync<WeatherData>(api, pars);
            Assert.IsNotNull(data);
            Assert.Equals("Liverpool", data.Name);
        }


        [Test]
        public void TestClientTestIsNotNull(HttpMessageHandler httpMessageHandler)
        {
            Assert.IsNotNull(httpMessageHandler);
            var client = new HttpClient(httpMessageHandler)
            {
                BaseAddress = new Uri(Constants.Constants.BaseUri)
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Assert.NotNull(client.BaseAddress);
            Assert.Equals(1, client.DefaultRequestHeaders.Accept.Count);
            Assert.NotNull(client);
        }

        [Test]
        public async Task<T> TestGetRequestAsync(string apiUrl, string pars)
        {
            var result = Activator.CreateInstance<T>();
            Assert.IsNotNull(result);
            Assert.IsTrue(Connection.NetworkConnected());

            Assert.IsNotNullOrEmpty(pars);
            Assert.IsNotNullOrEmpty(apiUrl);
            Assert.IsNotNullOrEmpty(Constants.Constants.BaseUri);
            Assert.IsNotNullOrEmpty(Constants.Constants.APIKey);

            var request = new Mock<HttpMessageHandler>(HttpMethod.Get, $"{Constants.Constants.BaseUri}{apiUrl}{pars}&appid={Constants.Constants.APIKey}");
            request.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(new MockWeatherData().ToString())
            });

            var httpClient = new HttpClient(request.Object);
            var send = await httpClient.GetAsync($"{Constants.Constants.BaseUri}{apiUrl}{pars}&appid={Constants.Constants.APIKey}");

            if (send.StatusCode == HttpStatusCode.OK)
            {
                var res = await send.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<T>(res);
            }
            
            Assert.IsNotNull(result);

            return result;
        }
    }
}
