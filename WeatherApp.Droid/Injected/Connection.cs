using System;
using System.IO;
using SQLite;
using WeatherApp.Interfaces;

namespace WeatherApp.Droid
{
    public class SQLConnection : ISqLiteConnectionFactory
    {
        readonly string Filename = "weather.db";

        public void GetConnection()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            path = Path.Combine(path, Filename);
            Constants.Constants.DBConnection =  new SQLiteConnection(path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        }
    }
}
