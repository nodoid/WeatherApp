using Newtonsoft.Json;
using NUnit.Framework;
using WeatherApps.Tests.Database;

namespace WeatherApp.Tests.Models
{
    [TestFixture]
    public class MockWeatherData :  SerializationTestBase
    {
        [Test]
        public void ValidLocationTest()
        {
            var text = @"{
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

            var result = JsonConvert.DeserializeObject<WeatherApp.Models.WeatherData>(text);
            Assert.AreEqual(420006353, result.Id);
            Assert.AreEqual("US", result.Sys.Country);
            Assert.AreEqual(280.37, result.Main.TempMin);
            Assert.AreEqual(200, result.Cod);
        }

        public void InvalidLocationTest()
        {
            var text = @"{
  ""message"":""accurate"",
""cod"":200,
""count"":0,
""list"":[]}
  }";

            var result = JsonConvert.DeserializeObject<WeatherApp.Models.WeatherFailedModel>(text);
            Assert.AreEqual(200, result.Cod);
            Assert.AreEqual("accurate", result.Message);
            Assert.AreEqual(0, result.Count);
        }
    }
}
