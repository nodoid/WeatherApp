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
            
        }

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