using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using UIKit;
using WeatherApp.Models;
using WeatherApp.ViewModels;
using Xamarin.Essentials;

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

            GetLocation();

            messenger.Register<BooleanMessage>(this, (m, t) =>
            {
                switch (t.Message)
                {
                    case "IsBusy":
                        Busy = t.BoolValue;
                        spinProgress.Hidden = Busy ? false : true;
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

            messenger.Register<StringMessage>(this, (m, t) => ShowError(t.Message));

            switchUseLocation.ValueChanged += (o, e) =>
            {
                if (ViewModel.CanUseGeoloc)
                    ViewModel.CanUseGeoloc = ((UISwitch)o).On;
            };
            switchSaveLocation.ValueChanged += (o, e) => ViewModel.UserOnStartup = ((UISwitch)o).On;
            btnResetLocation.TouchUpInside += (o, e) => ViewModel.ResetLocation = true;
            btnReset.TouchUpInside += (o, e) => ViewModel.ResetData = true;

            btnGetWeather.TouchUpInside += (o, e) =>
            {
                if (ViewModel.CanUseGeoloc)
                    ViewModel.GetDataForLocation.Execute(null);
                else
                    ViewModel.GetDataForCity.Execute(null);
            };

            txtCity.ValueChanged += (o, e) => ViewModel.City = ((UITextView)o).Text;
            txtState.ValueChanged += (o, e) => ViewModel.State = ((UITextView)o).Text;
            txtCountry.ValueChanged += (o, e) => ViewModel.Country = ((UITextView)o).Text;
            lblPlace.Text = lblGeolocation.Text = "";

            ViewModel.Setup();

            switchSaveLocation.On = ViewModel.UserOnStartup;
            switchUseLocation.On = ViewModel.CanUseGeoloc;

            spinProgress.Hidden = true;

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            lblPlace.Text = ViewModel.PlaceName;
            lblGeolocation.Text = ViewModel.Geoloc;
            switchUseLocation.On = ViewModel.CanUseGeoloc;
        }

        void ShowLightboxDialog()
        {
            viewResults.Hidden = false;
            btnOK.TouchUpInside += (o, e) => DismissLightbox();
            lblSky.Text = WeatherData.Weather[0].Description;
            lblTemp.Text = (WeatherData.Main.Temp - 273.15).ToString("n2");
            lblTempMin.Text = (WeatherData.Main.TempMin - 273.15).ToString("n2");
            lblTempMax.Text = (WeatherData.Main.TempMax - 273.15).ToString("n2");
            lblPressure.Text = $"{WeatherData.Main.Pressure}";
            lblHumid.Text = $"{WeatherData.Main.Humidity}";
            lblSunUp.Text = $"{ConvertFromEpoch(WeatherData.Sys.Sunrise)}";
            lblSunset.Text = $"{ConvertFromEpoch(WeatherData.Sys.Sunset)}";
            lblSpeed.Text = $"{WeatherData.Wind.Speed}";
            lblDegrees.Text = $"{WeatherData.Wind.Deg}";
        }

        void DismissLightbox()
        {
            viewResults.Hidden = true;
            ViewModel.ShowData = false;
        }

        string ConvertFromEpoch(int epoch) => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(epoch).ToShortTimeString();

        void GetLocation()
        {
            Task.Run(() =>
                BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                        if (status == PermissionStatus.Granted)
                        {
                            messenger.Send<BooleanMessage>(new BooleanMessage { BoolValue = true, Message = "Location" });
                            ViewModel.Location = await Geolocation.GetLastKnownLocationAsync();
                            if (ViewModel.Location == null)
                            {
                                ViewModel.Location = await Geolocation.GetLocationAsync(new GeolocationRequest
                                {
                                    DesiredAccuracy = GeolocationAccuracy.Medium,
                                    Timeout = TimeSpan.FromSeconds(15)
                                });
                                ViewModel.CanUseGeoloc = true;
                            }
                        }
                        else
                        {
                            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                            if (status == PermissionStatus.Granted)
                            {
                                messenger.Send<BooleanMessage>(new BooleanMessage { BoolValue = true, Message = "Location" });
                                ViewModel.Location = await Geolocation.GetLastKnownLocationAsync();
                                if (ViewModel.Location == null)
                                {
                                    ViewModel.Location = await Geolocation.GetLocationAsync(new GeolocationRequest
                                    {
                                        DesiredAccuracy = GeolocationAccuracy.Medium,
                                        Timeout = TimeSpan.FromSeconds(15)
                                    });
                                    ViewModel.CanUseGeoloc = true;
                                }
                            }
                            else
                            {
                                messenger.Send<BooleanMessage>(new BooleanMessage { BoolValue = false, Message = "Location" });
                                ViewModel.CanUseGeoloc = false;
                            }
                        }
                    }
                    catch (FeatureNotSupportedException fnsEx)
                    {
#if DEBUG
                        Console.WriteLine($"Not supported : {fnsEx.Message}--{fnsEx.InnerException?.Message}");
#endif
                    }
                    catch (PermissionException pEx)
                    {
#if DEBUG
                        Console.WriteLine($"Failed permission - {pEx.Message}--{pEx.InnerException?.Message}");
#endif
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex.ToString());
                    }
                }));
        }

        void ShowError(string message = "")
        {
            var alert = UIAlertController.Create("Error", string.IsNullOrEmpty(message) ? "The City and Country must be filled in" : message, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
            alert.PresentViewController(alert, animated: true, completionHandler: null); ;
        }

        public override void DidReceiveMemoryWarning ()
        {
            base.DidReceiveMemoryWarning ();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}