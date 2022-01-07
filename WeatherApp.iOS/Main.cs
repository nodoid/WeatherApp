using System;
using UIKit;

namespace WeatherApp.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            try
            {
                UIApplication.Main(args, null, typeof(AppDelegate));
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error : {ex.Message} -- {ex.InnerException?.Message}");
            }
        }
    }
}