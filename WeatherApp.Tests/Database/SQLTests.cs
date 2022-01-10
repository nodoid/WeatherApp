using NSubstitute;
using NUnit.Framework;
using SQLite;
using System.Linq;
using WeatherApp.Database;
using WeatherApp.Interfaces;

namespace WeatherApps.Tests.Database
{
    [TestFixture]
    public class SqLiteRepositoryTests
    {
        ISqLiteConnectionFactory connectionFactory;
        string testDatabase = ":memory:";
        SQLiteConnection connection;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            connectionFactory = Substitute.For<ISqLiteConnectionFactory>();
            connection = new SQLiteConnection(testDatabase);
            connectionFactory.GetConnection();
        }

        [SetUp]
        public void SetUp()
        {
            connection.BeginTransaction();
        }

        [Test]
        public void TestSavingUserSettings()
        {
            var sqliteRepo = new SqLiteRepository();
            sqliteRepo.SaveData(new WeatherApp.Models.UserSettings
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
            var count = sqliteRepo.Count<WeatherApp.Models.UserSettings, string>("Name", "Liverpool");
            Assert.AreEqual(1, count);
        }

        [Test]
        public void TestDeletingStoredCards()
        {
            var sqliteRepo = new SqLiteRepository();
            var data = new WeatherApp.Models.UserSettings
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
            };

            sqliteRepo.SaveData(data);

            sqliteRepo.Delete(data);
            var cards = sqliteRepo.GetList<WeatherApp.Models.UserSettings>();
            Assert.AreEqual(0, cards.Count());
        }

        [Test]
        public void TestAreRetrievingData()
        {
            var sqliteRepo = new SqLiteRepository();
            sqliteRepo.SaveData(new WeatherApp.Models.UserSettings
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

            var receipts = sqliteRepo.GetList<WeatherApp.Models.UserSettings>();
            Assert.AreEqual(1, receipts.Count());
            var first = receipts.First();
            Assert.AreEqual("Liverpool", first.CityName);
            Assert.AreEqual(53.243809, first.LastLat);
            Assert.AreEqual(false, first.AutoOnStart);
        }
    }
}
