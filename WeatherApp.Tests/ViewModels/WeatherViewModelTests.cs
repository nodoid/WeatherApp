using NSubstitute;
using NUnit.Framework;
using System.Threading.Tasks;
using WeatherApp.Database;
using WeatherApp.Interfaces;
using WeatherApp.Models;
using WeatherApp.Tests.Helpers;

namespace WeatherApp.Tests.ViewModels
{
    [TestFixture]
    public class WeatherViewModelTests
    {
        IWeatherService webservice = Substitute.For<IWeatherService>();
        WeatherData WeatherData { get; set; }
        SqLiteRepository sqliteRepo { get; set; }

        [SetUp]
        public void SetupDatabase()
        {
            sqliteRepo = new SqLiteRepository();
            var count = sqliteRepo.Count<UserSettings>();
            if (count != 0)
                sqliteRepo.DeleteAll();
        }

        [Test]
        public void AddUserSettings_ToDB()
        {
            sqliteRepo.SaveData(new UserSettings
            {
                AutoOnStart = false,
                CanUseGeolocation = true,
                CityName = "Mountain View",
                Country = "US",
                Id = 3,
                IsUSState = true,
                USState = "",
                LastLat = 37.39,
                LastLng = -122.08
            });
            var data = sqliteRepo.Count<UserSettings>();

            Assert.IsNotNull(data);
        }

        [Test]
        public void Test_UserSettings_NotNull()
        {
            var user = sqliteRepo.GetData<UserSettings, int>("Id", 0);
            Assert.IsNotNull(user);
            Assert.AreEqual("Mountain View", user.CityName);
            Assert.AreEqual("0", user.Id);
            Assert.AreEqual(37.39, user.LastLat);
        }

        [Test]
        public async Task Test_UserSettings_NotNull_CallGetCity()
        {
            var user = sqliteRepo.GetData<UserSettings, int>("Id", 0);

            var state = string.IsNullOrEmpty(user.USState) ? "" : user.USState;

            if (!string.IsNullOrEmpty(user.CityName) && !string.IsNullOrEmpty(user.Country))
            {
                var pt = user.CityName.LastIndexOf(' ');
                if (pt != -1)
                    user.CityName = user.CityName.Substring(0, pt);
                pt = user.Country.LastIndexOf(' ');
                if (pt != -1)
                    user.Country = user.Country.Substring(0, pt);

                var data = await webservice.GetWeatherForCity(user.CityName, user.Country, state);
                WeatherData = data;
                Assert.IsNotNull(data);
                Assert.Equals("Mountain View", data.Name);
            }
        }

        [Test]
        public void Test_UserSettings_NotNull_CallFromLocation()
        {
            var user = sqliteRepo.GetData<UserSettings, int>("Id",0 );

            var data = new MockWeatherDataSetup().Setup_WeatherData();
            if (data != null)
            {
                WeatherData = data;
                Assert.IsNotNull(data);
                Assert.Equals(53.243809, data.Coord.Lat);
            }
        }

        [Test]
        public void Test_SaveSettings_NameIsMountainView()
        {
            if (WeatherData.Name.Equals("Mountain View")) 
            {
                var userData = new UserSettings
                {
                    AutoOnStart = false,
                    CanUseGeolocation = true,
                    CityName = "Mountain View",
                    Country = "US",
                    Id = 3,
                    IsUSState = true,
                    USState = "",
                    LastLat = 37.39,
                    LastLng = -122.08
                };

                sqliteRepo.SaveData(userData);
                var dataBack = sqliteRepo.GetData<UserSettings, int>("Id", 3);
                Assert.IsNotNull(dataBack);
                Assert.Equals(-2.584058, dataBack.LastLng);
            }
        }

        [Test]
        public void Test_SaveSettings_NameIsNotMountainView()
        {
            Assert.AreNotEqual(WeatherData.Name, "Everton");
        }

        [Test]
        public void Test_UserSettings_Null()
        {
            var user = sqliteRepo.GetData<UserSettings, int>("Id", 1);
            Assert.IsNull(user);
        }
    }
}