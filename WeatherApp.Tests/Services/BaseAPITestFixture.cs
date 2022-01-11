using AutoFixture;
using NUnit.Framework;

namespace WeatherApps.Tests.Services
{
    [TestFixture]
    public class BaseAPITestFixture
    {
        protected Fixture Fixture { get; set; }

        [SetUp]
        public void Set_up_test_context()
        {
            this.Fixture = new Fixture();
        }
    }
}