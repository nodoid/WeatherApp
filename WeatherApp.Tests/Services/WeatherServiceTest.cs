﻿using Moq;
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

        [SetUp]
        public WeatherData Setup_WeatherData()
        {
            var data = @"{
  ""coord"": {
    ""lon"": -122.08,
    ""lat"": 37.39
  },
  ""weather"": [
    {
      ""id"": 800,
      ""main"": ""Clear"",
      ""description"": ""clear sky"",
      ""icon"": ""01d""
    }
  ],
  ""base"": ""stations"",
  ""main"": {
    ""temp"": 282.55,
    ""feels_like"": 281.86,
    ""temp_min"": 280.37,
    ""temp_max"": 284.26,
    ""pressure"": 1023,
    ""humidity"": 100
  },
  ""visibility"": 16093,
  ""wind"": {
    ""speed"": 1.5,
    ""deg"": 350
  },
  ""clouds"": {
    ""all"": 1
  },
  ""dt"": 1560350645,
  ""sys"": {
    ""type"": 1,
    ""id"": 5122,
    ""message"": 0.0139,
    ""country"": ""US"",
    ""sunrise"": 1560343627,
    ""sunset"": 1560396563
  },
  ""timezone"": -25200,
  ""id"": 420006353,
  ""name"": ""Mountain View"",
  ""cod"": 200
  }";
            return JsonConvert.DeserializeObject<WeatherData>(data);
        }
        

        [Test]
        public void Test_GetWeatherForCity(string city, string country, string state = "")
        {
            var api = "q=";
            var pars = string.IsNullOrEmpty(state) ? $"{city},{country}" : $"{city},{state},{country}";
            var data = Setup_WeatherData();
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
            var data = Setup_WeatherData();
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
        public T CreateInstance_OfT<T> (T objectType)
        {
            var result = Activator.CreateInstance<T>();
            Assert.IsNotNull(result);

            return result;
        }

        [Test]
        public async Task Test_GetRequestAsync<T>(string apiUrl, string pars)
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
        }

        [Test]
        public async Task Test_GetRequestAsync_Returns401<T>(string apiUrl, string pars)
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
            var send = await httpClient.GetAsync($"{Constants.Constants.BaseUri}{apiUrl}{pars}&appid={Constants.Constants.APIKey}");

            Assert.Equals(401, send.StatusCode);
        }

        [Test]
        public async Task Test_GetRequestAsync_Returns404<T>(string apiUrl, string pars)
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
            var send = await httpClient.GetAsync($"1{Constants.Constants.BaseUri}{apiUrl}{pars}&appid={Constants.Constants.APIKey}");

            Assert.Equals(404, send.StatusCode);
        }

        [Test]
        [ExpectedException(typeof(JsonException))]
        public async Task Test_GetRequestAsync_Exception<T>(string apiUrl, string pars)
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
        }
    }
}
