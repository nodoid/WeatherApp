using NUnit.Framework;
using AutoFixture;

namespace WeatherApp.Tests.ViewModels
{


    [TestFixture]
    public class ViewModelTestFixtureBase
    {
        protected Fixture Fixture { get; set; }

        [SetUp]
        public void Set_up_test_context()
        {
            this.Fixture = new Fixture();
        }
    }
}