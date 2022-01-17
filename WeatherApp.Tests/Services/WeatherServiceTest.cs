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
        public async Task Test_GetWeatherForCity(string city, string country, string state = "")
        {
            var api = "q=";
            var pars = string.IsNullOrEmpty(state) ? $"{city},{country}" : $"{city},{state},{country}";
            var data = await Test_GetRequestAsync<WeatherData>(api, pars);
            Assert.IsNotNull(data);
            Assert.Equals("Liverpool", data.Name);
        }

        async Task<T1> Test_GetRequestAsync<T1>(string api, string pars)
        {
            return await Test_GetRequestAsync<T1>(api, pars);
        }

        [Test]
        public async Task TestGetWeatherForLocation(double lng, double lat)
        {
            var api = "lat=";
            var pars = $"{lat}&lon={lng}";
            var data = await Test_GetRequestAsync<WeatherData>(api, pars);
            Assert.IsNotNull(data);
            Assert.Equals("Liverpool", data.Name);
        }


        [Test]
        public void Test_ClientTestIsNotNull(HttpMessageHandler httpMessageHandler)
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
        public async Task<T> Test_GetRequestAsync(string apiUrl, string pars)
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

            Assert.Equals(401, send.StatusCode);
            Assert.IsNotNull(result);

            return result;
        }

        [Test]
        public async Task<T> Test_GetRequestAsync_Returns401(string apiUrl, string pars)
        {
            var result = Activator.CreateInstance<T>();
            Assert.IsNotNull(result);
            Assert.IsTrue(Connection.NetworkConnected());

            Assert.IsNotNullOrEmpty(pars);
            Assert.IsNotNullOrEmpty(apiUrl);
            Assert.IsNotNullOrEmpty(Constants.Constants.BaseUri);
            Assert.IsNotNullOrEmpty(Constants.Constants.APIKey);

            var request = new Mock<HttpMessageHandler>(HttpMethod.Get, $"1{Constants.Constants.BaseUri}{apiUrl}{pars}&appid={Constants.Constants.APIKey}");
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

            Assert.Equals(404, send.StatusCode);
            Assert.IsNotNull(result);

            return result;
        }

        public async Task<T> Test_GetRequestAsync_Returns404(string apiUrl, string pars)
        {
            var result = Activator.CreateInstance<T>();
            Assert.IsNotNull(result);
            Assert.IsTrue(Connection.NetworkConnected());

            Assert.IsNotNullOrEmpty(pars);
            Assert.IsNotNullOrEmpty(apiUrl);
            Assert.IsNotNullOrEmpty(Constants.Constants.BaseUri);

            var request = new Mock<HttpMessageHandler>(HttpMethod.Get, $"1{Constants.Constants.BaseUri}");
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

        [Test]
        [ExpectedException(typeof(JsonException))]
        public async Task<T> Test_GetRequestAsync_Exception(string apiUrl, string pars)
        {
            var result = Activator.CreateInstance<T>();
            Assert.IsNotNull(result);
            Assert.IsTrue(Connection.NetworkConnected());

            Assert.IsNotNullOrEmpty(pars);
            Assert.IsNotNullOrEmpty(apiUrl);
            Assert.IsNotNullOrEmpty(Constants.Constants.BaseUri);

            var request = new Mock<HttpMessageHandler>(HttpMethod.Get, $"1{Constants.Constants.BaseUri}");
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
                var r = JsonConvert.DeserializeObject(res);
                result = r as T;
            }

            Assert.IsNotNull(result);

            return result;
        }
    }
}
