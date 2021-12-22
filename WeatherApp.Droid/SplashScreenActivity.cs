using System.Threading;
using Android.App;
using Android.Content.PM;
using Android.OS;

namespace WeatherApp.Droid
{
    [Activity(Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true,
        ConfigurationChanges = ConfigChanges.ScreenSize,ScreenOrientation =ScreenOrientation.Portrait)]
    public class SplashScreenActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Thread.Sleep(500);

            StartActivity(typeof(MainActivity));
        }
    }
}
