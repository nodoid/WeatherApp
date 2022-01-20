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
using WeatherApp.Tests.Helpers;
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
        public void Test_GetWeatherForCity(string city, string country, string state = "")
        {
            var api = "q=";
            var pars = string.IsNullOrEmpty(state) ? $"{city},{country}" : $"{city},{state},{country}";
            var data = new MockWeatherDataSetup().Setup_WeatherData();
            Assert.IsNotNull(data);
            Assert.NotNull(api);
            Assert.NotNull(pars);
            Assert.Equals("Mountain View", data.Name);
        }

        [Test]
        public void Test_GetWeatherForLocation(double lng, double lat)
        {
            var api = "lat=";
            var pars = $"{lat}&lon={lng}";
            var data = new MockWeatherDataSetup().Setup_WeatherData();
            Assert.IsNotNull(data);
            Assert.NotNull(pars);
            Assert.NotNull(api);
            Assert.Equals(37.39, data.Coord.Lat);
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
        public void CreateInstance_OfT<T> (T objectType)
        {
            var result = Activator.CreateInstance<T>();
            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_GetRequestAsync<T>(string apiUrl, string pars)
        {
            var result = new MockWeatherData();

            var request = new Mock<HttpMessageHandler>(HttpMethod.Get, $"{Constants.Constants.BaseUri}{apiUrl}{pars}&appid={Constants.Constants.APIKey}");
            request.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(new MockWeatherData().ToString())
                });

            var httpClient = new HttpClient(request.Object);
            var send = httpClient.GetAsync($"{Constants.Constants.BaseUri}{apiUrl}{pars}&appid={Constants.Constants.APIKey}").Result;

            if (send.StatusCode == HttpStatusCode.OK)
            {
                var res = send.Content.ReadAsStringAsync().Result;
                result = JsonConvert.DeserializeObject<MockWeatherData>(res);
            }

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_GetRequestAsync_Returns401<T>(string apiUrl, string pars)
        {
            var request = new Mock<HttpMessageHandler>(HttpMethod.Get, $"1{Constants.Constants.BaseUri}{apiUrl}{pars}&appid={Constants.Constants.APIKey}");
                request.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Content = new StringContent(new MockWeatherData().ToString())
                });

            var httpClient = new HttpClient(request.Object);
            var send = httpClient.GetAsync($"{Constants.Constants.BaseUri}{apiUrl}{pars}&appid={Constants.Constants.APIKey}").Result;

            Assert.Equals(401, send.StatusCode);
        }

        [Test]
        public void Test_GetRequestAsync_Returns404<T>(string apiUrl, string pars)
        {
            var request = new Mock<HttpMessageHandler>(HttpMethod.Get, $"1{Constants.Constants.BaseUri}");
            request.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Content = new StringContent(new MockWeatherData().ToString())
                });

            var httpClient = new HttpClient(request.Object);
            var send = httpClient.GetAsync($"1{Constants.Constants.BaseUri}{apiUrl}{pars}&appid={Constants.Constants.APIKey}").Result;

            Assert.Equals(404, send.StatusCode);
        }

        [Test]
        [ExpectedException(typeof(JsonException))]
        public void Test_GetRequestAsync_Exception<T>(string apiUrl, string pars)
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
            var send = httpClient.GetAsync($"{Constants.Constants.BaseUri}{apiUrl}{pars}&appid={Constants.Constants.APIKey}").Result;

            if (send.StatusCode == HttpStatusCode.OK)
            {
                var res = send.Content.ReadAsStringAsync().Result;
                var r = JsonConvert.DeserializeObject(res);
                result = (T)r;
            }
        }
    }
}
