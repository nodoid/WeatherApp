using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System;
using UIKit;
using WeatherApp.Models;
using WeatherApp.ViewModels;

namespace WeatherApp.iOS
{
    public partial class ViewController : UIViewController
    {
        IMessenger messenger => (IMessenger)Startup.ServiceProvider.GetService(typeof(IMessenger));
        WeatherViewModel ViewModel { get; set; }

        bool Busy { get; set; }
        WeatherData WeatherData { get; set; }

        public ViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            ViewModel = AppDelegate.Service.GetService<WeatherViewModel>();

            messenger.Register<BooleanMessage>(this, (m, t) =>
            {
                switch (t.Message)
                {
                    case "IsBusy":
                        Busy = t.BoolValue;
                        
                        break;
                    case "HasData":
                        WeatherData = ViewModel.WeatherData;
                        if (!string.IsNullOrEmpty(WeatherData.Sys.Country))
                            ShowLightboxDialog();
                        else
                            ShowError("The weather data returned was empty");
                        break;
                    case "Error":
                        ShowError();
                        break;
                }
            });

            
        }

        void ShowLightboxDialog()
        {
            btnOK.TouchUpInside += (o, e) => DismissLightbox();
            lblSky.Text = WeatherData.Weather[0].Description;
            lblTemp.Text = (WeatherData.Main.Temp - 273.15).ToString("n2");

        }

        void DismissLightbox()
        {
            viewResults.Hidden = true;
            ViewModel.ShowData = false;
        }

        string ConvertFromEpoch(int epoch) => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(epoch).ToShortTimeString();

        public override void DidReceiveMemoryWarning ()
        {
            base.DidReceiveMemoryWarning ();
            // Release any cached data, images, etc that aren't in use.
        }

        void ShowError(string message = "")
        {
            var alert = UIAlertController.Create("Error", string.IsNullOrEmpty(message) ? "The City and Country must be filled in" : message, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
            alert.PresentViewController(alert, animated: true, completionHandler: null); ;
        }
    }
}