using NUnit.Framework;
using System.Threading.Tasks;

namespace Tests
{
    public class CityAndCountryTests : SerializationTestBase
    {
        [TestFixture]
        public class PostcodeTests : SerializationTestBase
        {
            [Test]
            public async Task TestCountryAndCityIsFalse()
            {
                var city = "Liverpool";
                var country = "West Berlin";

                var data = await new WeatherApp.Services.WeatherService().GetWeatherForCity(city, country);

                Assert.AreEqual(true, string.IsNullOrEmpty(data.Name));
                Assert.AreEqual(false, data.Weather != null);
            }

            [Test]
            public async Task TestCountryAndCityIsTrue()
            {
                var city = "Liverpool";
                var country = "UK";

                var data = await new WeatherApp.Services.WeatherService().GetWeatherForCity(city, country);

                Assert.AreEqual(true, !string.IsNullOrEmpty(data.Name));
                Assert.AreEqual(false, data.Weather != null);
            }

            [Test]
            public async Task TestGeoDataCountryAndCityIsFalse()
            {
                var lat = 53.243809;
                var lng = 0.123;

                var data = await new WeatherApp.Services.WeatherService().GetWeatherForLocation(lng, lat);

                Assert.AreEqual(false, data.Name.Equals("Liverpool")); ;
                Assert.AreEqual(false, data.Weather != null);
            }

            [Test]
            public async Task TestGeodataCountryAndCityIsTrue()
            {
                var lat = 53.243809;
                var lng = 0.123;

                var data = await new WeatherApp.Services.WeatherService().GetWeatherForLocation(lng, lat);

                Assert.AreEqual(true, data.Name.Equals("Liverpool"));
                Assert.AreEqual(false, data.Weather != null);
            }
        }
    }
}
