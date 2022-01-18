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
    public class WeatherServiceTest
    {
        IConnection Connection;
        HttpMessageHandler httpMessageHandler;

        [SetUp]
        public void Setup_WeatherServiceTest()
        {
            Connection = Substitute.For<IConnection>();
            Connection.NetworkConnected().Returns(true);
            httpMessageHandler = Substitute.For<HttpMessageHandler>();
        }

        [Test]
        public async Task Test_GetWeatherForCity(string city, string country, string state = "")
        {
            var api = "q=";
            var pars = string.IsNullOrEmpty(state) ? $"{city},{country}" : $"{city},{state},{country}";
            var data = await Test_GetRequestAsync<WeatherData>(api, pars);
            Assert.IsNotNull(data);
            Assert.Equals("Liverpool", data.Name);
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
        public void Test_HttpMessageHandler_IsNull(HttpMessageHandler httpMessageHandler)
        {
            Assert.IsNull(httpMessageHandler);
        }

        [Test]
        public void Test_HttpMessageHandler_IsNotNull(HttpMessageHandler httpMessageHandler)
        {
            Assert.IsNotNull(httpMessageHandler);
        }

        [Test]
        public void Test_ClientTestIsNotNull(HttpMessageHandler httpMessageHandler)
        {
            var client = new HttpClient(httpMessageHandler)
            {
                BaseAddress = new Uri(Constants.Constants.BaseUri)
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            Assert.NotNull(client);
            Assert.NotNull(client.BaseAddress);
            Assert.Equals(1, client.DefaultRequestHeaders.Accept.Count);   
        }

        [Test]
        public T CreateInstance_OfT<T> (T objectType)
        {
            var result = Activator.CreateInstance<T>();
            Assert.IsNotNull(result);

            return result;
        }

        [Test]
        public async Task<T> Test_GetRequestAsync<T>(string apiUrl, string pars)
        {
            var result = Activator.CreateInstance<T>(); ;

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

        [Test]
        public async Task<T> Test_GetRequestAsync_Returns401<T>(string apiUrl, string pars)
        {
            var result = Activator.CreateInstance<T>();

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

        public async Task<T> Test_GetRequestAsync_Returns404<T>(string apiUrl, string pars)
        {
            var result = Activator.CreateInstance<T>();

            var request = new Mock<HttpMessageHandler>(HttpMethod.Get, $"1{Constants.Constants.BaseUri}");
            request.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(new MockWeatherData().ToString())
                });

            var httpClient = new HttpClient(request.Object);
            var send = await httpClient.GetAsync($"1{Constants.Constants.BaseUri}{apiUrl}{pars}&appid={Constants.Constants.APIKey}");

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
        public async Task<T> Test_GetRequestAsync_Exception<T>(string apiUrl, string pars)
        {
            var result = Activator.CreateInstance<T>();

            var request = new Mock<HttpMessageHandler>(HttpMethod.Get, $"{Constants.Constants.BaseUri}");
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
                result = (T)r;
            }

            Assert.IsNotNull(result);

            return result;
        }
    }
}
