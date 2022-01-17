using Moq;
using NUnit.Framework;
using System.Linq;
using WeatherApp.Helpers;

namespace WeatherApp.Tests.Helpers
{
    [TestFixture]
    public class HttpClientTests
    {
        [Test]
        public void TestSetup()
        {
            var test = new HttpClientHelper().SetupClient();
            Assert.IsNotNull(test);
        }

        [Test]
        public void TestSetupTimeouWithValueIsMinutesFalse()
        {
            var test = new HttpClientHelper().SetupClient(1, false);
            Assert.IsNotNull(test);
            Assert.Equals(1, test.Timeout);
        }

        [Test]
        public void TestSetupTimeouWithValueIsMinutesTrue()
        {
            var test = new HttpClientHelper().SetupClient(2);
            Assert.IsNotNull(test);
            Assert.Equals(2, test.Timeout);
        }

        [Test]
        public void TestSetupWithAcceptMediaNoType()
        {
            var test = new HttpClientHelper().SetupClient(3, false, true);
            Assert.IsNotNull(test);
            Assert.Equals(3, test.Timeout);
            Assert.IsEmpty(test.DefaultRequestHeaders.Accept);
        }

        [Test]
        public void TestSetupWithAcceptMediaAndType()
        {
            var test = new HttpClientHelper().SetupClient(4, false, true, "application/json");
            Assert.IsNotNull(test);
            Assert.Equals(3, test.Timeout);
            Assert.Equals(1, test.DefaultRequestHeaders.Accept.Count);
            Assert.Equals("application/json", test.DefaultRequestHeaders.Accept.First());
        }
    }
}
