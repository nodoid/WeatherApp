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

        bool Busy { get; set; }
        WeatherData WeatherData { get; set; }

        public ViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
            messenger.Register<BooleanMessage>(this, (m, t) =>
            {
                switch (t.Message)
                {
                    case "IsBusy":
                        Busy = t.BoolValue;
                        break;
                    case "HasData":
                        WeatherData = AppDelegate.Service.GetService<WeatherViewModel>().WeatherData;
                        break;
                }
            });
            // Perform any additional setup after loading the view, typically from a nib.
        }

        public override void DidReceiveMemoryWarning ()
        {
            base.DidReceiveMemoryWarning ();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}