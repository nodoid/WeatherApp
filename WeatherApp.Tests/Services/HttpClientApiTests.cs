using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using AutoFixture;
using WeatherApp.Tests.Interfaces;
using WeatherApp.Models;
using WeatherApp.Tests.Services;

namespace WeatherApps.Tests.Services
{
    public class HttpClientApiTests : BaseAPITestFixture
    {
        [Test]
        public async Task GetRequestAsync_with_404_returns_null()
        {
            var connection = Substitute.For<IConnection>();

            var responseMessage = new HttpResponseMessage();
            var mockVRMLookupResponse = this.Fixture.Create<FakeResponseObject>();
            var mockApiResponse = JsonConvert.SerializeObject(mockVRMLookupResponse);
            responseMessage.Content = new FakeHttpContent(mockApiResponse);
            responseMessage.StatusCode = HttpStatusCode.NotFound;
            var messageHandler = new FakeHttpMessageHandler(responseMessage);
            var sut = new HttpClientApi<FakeResponseObject>(messageHandler, connection);
            var data = await sut.GetRequestAsync(string.Empty, string.Empty);
            Assert.IsNull(data);
        }

        [Test]
        public async Task GetRequestAsync_returns_correct_object()
        {
            var connection = Substitute.For<IConnection>();

            connection.NetworkConnected().Returns(true);
            var responseMessage = new HttpResponseMessage();
            var mockVRMLookupResponse = this.Fixture.Create<FakeResponseObject>();
            var mockApiResponse = JsonConvert.SerializeObject(mockVRMLookupResponse);
            responseMessage.Content = new FakeHttpContent(mockApiResponse);
            var messageHandler = new FakeHttpMessageHandler(responseMessage);
            var sut = new HttpClientApi<FakeResponseObject>(messageHandler, connection);

            var data = await sut.GetRequestAsync(string.Empty, string.Empty);
            Assert.AreEqual(mockVRMLookupResponse.StringProperty, data.StringProperty);
            Assert.AreEqual(mockVRMLookupResponse.ByteProperty, data.ByteProperty);
            Assert.AreEqual(mockVRMLookupResponse.SByteProperty, data.SByteProperty);
            Assert.AreEqual(mockVRMLookupResponse.IntProperty, data.IntProperty);
            Assert.AreEqual(mockVRMLookupResponse.UIntProperty, data.UIntProperty);
            Assert.AreEqual(mockVRMLookupResponse.ShortProperty, data.ShortProperty);
            Assert.AreEqual(mockVRMLookupResponse.UShortProperty, data.UShortProperty);
            Assert.AreEqual(mockVRMLookupResponse.DoubleProperty, data.DoubleProperty);
            Assert.AreEqual(mockVRMLookupResponse.FloatProperty, data.FloatProperty);
            Assert.AreEqual(mockVRMLookupResponse.LongProperty, data.LongProperty);
            Assert.AreEqual(mockVRMLookupResponse.BoolProperty, data.BoolProperty);
            Assert.AreEqual(mockVRMLookupResponse.CharProperty, data.CharProperty);
            Assert.AreEqual(mockVRMLookupResponse.DecimalProperty, data.DecimalProperty);
        }

        [Test]
        [ExpectedException(typeof(HttpRequestException))]
        public async Task GetRequestAsync_throws_exception_on_failure_status_code()
        {
            var connection = Substitute.For<IConnection>();

            connection.NetworkConnected().Returns(true);
            var responseMessage = new HttpResponseMessage { StatusCode = HttpStatusCode.ServiceUnavailable };
            var messageHandler = new FakeHttpMessageHandler(responseMessage);

            var sut = new HttpClientApi<WeatherData>(messageHandler, connection);

            await sut.GetRequestAsync(string.Empty, string.Empty);
        }
    }
}