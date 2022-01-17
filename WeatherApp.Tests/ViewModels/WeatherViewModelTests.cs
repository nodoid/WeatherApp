using NSubstitute;
using NUnit.Framework;
using System.Threading.Tasks;
using WeatherApp.Database;
using WeatherApp.Interfaces;
using WeatherApp.Models;

namespace WeatherApp.Tests.ViewModels
{
    [TestFixture]
    public class WeatherViewModelTests : ViewModelTestFixtureBase
    {
        readonly IWeatherService webservice = Substitute.For<IWeatherService>();
        WeatherData WeatherData { get; set; }
        SqLiteRepository sqliteRepo { get; set; }

        [SetUp]
        public void SetupViewModelTest()
        {
            sqliteRepo = new SqLiteRepository();
            sqliteRepo.SaveData(new UserSettings
            {
                AutoOnStart = false,
                CanUseGeolocation = true,
                CityName = "Liverpool",
                Country = "UK",
                Id = 0,
                IsUSState = false,
                USState = "",
                LastLat = 53.243809,
                LastLng = -2.584058
            });
            var test = sqliteRepo.GetData<UserSettings, int>("Id", 0);
        }

        [Test]
        public void TestUserSettingsNotNull()
        {
            var user = sqliteRepo.GetData<UserSettings, int>("Id", 0);
            Assert.IsNotNull(user);
            Assert.AreEqual("Liverpool", user.CityName);
            Assert.AreEqual("0", user.Id);
            Assert.AreEqual(53.243809, user.LastLat);
        }

        [Test]
        public async Task TestUserSettingsNotNullCallGetCity()
        {
            var user = sqliteRepo.GetData<UserSettings, int>("Id", 0);
            Assert.IsNotNull(user);
            Assert.AreEqual("Liverpool", user.CityName);
            Assert.AreEqual("0", user.Id);
            Assert.AreEqual(53.243809, user.LastLat);

            var state = string.IsNullOrEmpty(user.USState) ? "" : user.USState;
            Assert.IsNullOrEmpty(state);

            if (!string.IsNullOrEmpty(user.CityName) && !string.IsNullOrEmpty(user.Country))
            {
                var pt = user.CityName.LastIndexOf(' ');
                if (pt != -1)
                    user.CityName = user.CityName.Substring(0, pt);
                pt = user.Country.LastIndexOf(' ');
                if (pt != -1)
                    user.Country = user.Country.Substring(0, pt);

                var data = await webservice.GetWeatherForCity(user.CityName, user.Country, state);
                Assert.IsNotNull(data);
                Assert.Equals("Liverpool", data.Name);
                WeatherData = data;
            }
        }

        [Test]
        public async Task TestUserSettingsNotNullCallFromLocation()
        {
            var user = sqliteRepo.GetData<UserSettings, int>("Id",0 );
            Assert.IsNotNull(user);
            Assert.AreEqual("Liverpool", user.CityName);
            Assert.AreEqual("0", user.Id);
            Assert.AreEqual(53.243809, user.LastLat);

            var data = await webservice.GetWeatherForLocation(user.LastLng, user.LastLng);
            if (data != null)
            {
                Assert.IsNotNull(data);
                Assert.Equals(53.243809, data.Coord.Lat);
                WeatherData = data;
            }
        }

        [Test]
        public void TestSaveSettings()
        {
            Assert.Equals("Liverppol", WeatherData.Name);
            if (WeatherData.Name.Equals("Liverpool")) 
            {
                var userData = new UserSettings
                {
                    AutoOnStart = false,
                    CanUseGeolocation = true,
                    CityName = "Liverpool",
                    Country = "UK",
                    Id = 3,
                    IsUSState = false,
                    USState = "",
                    LastLat = 53.243809,
                    LastLng = -2.584058
                };

                sqliteRepo.SaveData(userData);
                var dataBack = sqliteRepo.GetData<UserSettings, int>("Id", 3);
                Assert.IsNotNull(dataBack);
                Assert.Equals(-2.584058, dataBack.LastLng);
            }
        }

        [Test]
        public void TestUserSettingsNull()
        {
            var user = sqliteRepo.GetData<UserSettings, int>("Id", 1);
            Assert.IsNull(user);
        }
    }
}