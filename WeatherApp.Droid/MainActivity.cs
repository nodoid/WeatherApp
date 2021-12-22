using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using WeatherApp.Models;
using WeatherApp.ViewModels;
using Xamarin.Essentials;
using AlertDialog = Android.App.AlertDialog;

namespace WeatherApp.Droid
{
    [Activity(Theme = "@style/AppTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {
        public static IServiceProvider Service { get; set; }

        IMessenger messenger => (IMessenger)Startup.ServiceProvider.GetService(typeof(IMessenger));
        bool Busy { get; set; }
        WeatherData WeatherData { get; set; }

        Dialog ModalDialog;
        ProgressBar ProgressBar;

        WeatherViewModel ViewModel { get; set; }
        BaseViewModel BaseViewModel { get; set; }

        Context context;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);

            context = this;

            new SQLConnection().GetConnection();
            Service = WeatherApp.Startup.Init();

            BaseViewModel = Service.GetService<BaseViewModel>();

            var connnect = Connectivity.NetworkAccess;
            BaseViewModel.IsConnected = connnect == NetworkAccess.Internet;
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;

            ViewModel = Service.GetService<WeatherViewModel>();

            GetLocation();

            messenger.Register<BooleanMessage>(this, (m, t) =>
            {
                switch (t.Message)
                {
                    case "IsBusy":
                        Busy = t.BoolValue;
                        ProgressBar.Visibility = Busy ? ViewStates.Visible : ViewStates.Invisible;
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

            SetContentView(Resource.Layout.activity_main);
            ProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar1);
            ProgressBar.Visibility = ViewStates.Invisible;

            var save = FindViewById<CheckBox>(Resource.Id.checkBox1);
            var loc = FindViewById<CheckBox>(Resource.Id.chkLocation);
            var go = FindViewById<Button>(Resource.Id.btnWeather);
            var reset = FindViewById<Button>(Resource.Id.btnReset);
            var clear = FindViewById<Button>(Resource.Id.btnResetData);
            var city = FindViewById<EditText>(Resource.Id.editCity);
            var country = FindViewById<EditText>(Resource.Id.editCountry);
            var state = FindViewById<EditText>(Resource.Id.editState);

            save.CheckedChange += (o, e) => ViewModel.UserOnStartup = e.IsChecked;
            reset.Click += (o, e) => ViewModel.ResetLocation = true;
            clear.Click += (o,e) => ViewModel.ResetData = true;

            loc.CheckedChange += (o, e) =>
            {
                if (ViewModel.CanUseGeoloc)
                    ViewModel.CanUseGeoloc = e.IsChecked;
            };

            go.Click += (o, e) =>
            {
                if (ViewModel.CanUseGeoloc)
                    ViewModel.GetDataForLocation.Execute(null);
                else
                    ViewModel.GetDataForCity.Execute(null);
            };

            city.TextChanged += (o, e) => ViewModel.City = e.Text.ToString();
            state.TextChanged += (o, e) => ViewModel.State = e.Text.ToString();
            country.TextChanged += (o, e) => ViewModel.Country = e.Text.ToString();

            ViewModel.Setup();

            save.Checked = ViewModel.UserOnStartup;
            loc.Checked = ViewModel.CanUseGeoloc;
        }

        void ShowError(string s = "")
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(context);
            if (!string.IsNullOrEmpty(s))
                builder.SetMessage(s);
            else
                builder.SetMessage("The City and Country must be filled in");
            builder.SetTitle("Error");
            builder.SetCancelable(false);
            builder.SetPositiveButton("OK", (object o, Android.Content.DialogClickEventArgs e) =>
            {
                builder.Dispose();
            });
            AlertDialog alert = builder.Create();
            alert.Show();
        }

        void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            BaseViewModel.IsConnected = Convert.ToBoolean(e.NetworkAccess);
        }

        void GetLocation()
        {
            var loc = Service.GetService<WeatherViewModel>();
            Task.Run(() =>
                RunOnUiThread(async () =>
                {
                    try
                    {
                        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                        if (status == PermissionStatus.Granted)
                        {
                            messenger.Send<BooleanMessage>(new BooleanMessage { BoolValue = true, Message = "Location" });
                            loc.Location = await Geolocation.GetLastKnownLocationAsync();
                            if (loc.Location == null)
                            {
                                loc.Location = await Geolocation.GetLocationAsync(new GeolocationRequest
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
                                loc.Location = await Geolocation.GetLastKnownLocationAsync();
                                if (loc.Location == null)
                                {
                                    loc.Location = await Geolocation.GetLocationAsync(new GeolocationRequest
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

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void ShowLightboxDialog()
        {
            ModalDialog = new Dialog(this, Resource.Style.lightbox_dialog);
            ModalDialog.SetContentView(Resource.Layout.ModalDialog);
            ((TextView)ModalDialog.FindViewById(Resource.Id.txtSky)).Text = WeatherData.Weather[0].Description;
            ((TextView)ModalDialog.FindViewById(Resource.Id.txtTemp)).Text = (WeatherData.Main.Temp - 273.15).ToString("n2");
            ((TextView)ModalDialog.FindViewById(Resource.Id.txtTempMin)).Text = (WeatherData.Main.TempMin - 273.15).ToString("n2");
            ((TextView)ModalDialog.FindViewById(Resource.Id.txtTempMax)).Text = (WeatherData.Main.TempMax - 273.15).ToString("n2");
            ((TextView)ModalDialog.FindViewById(Resource.Id.txtPressure)).Text = $"{WeatherData.Main.Pressure}";
            ((TextView)ModalDialog.FindViewById(Resource.Id.txtHumidity)).Text = $"{WeatherData.Main.Humidity}";
            ((TextView)ModalDialog.FindViewById(Resource.Id.txtSunrise)).Text = $"{ConvertFromEpoch(WeatherData.Sys.Sunrise)}";
            ((TextView)ModalDialog.FindViewById(Resource.Id.txtSunset)).Text = $"{ConvertFromEpoch(WeatherData.Sys.Sunset)}";
            ((TextView)ModalDialog.FindViewById(Resource.Id.txtWind)).Text = $"{WeatherData.Wind.Speed}";
            ((TextView)ModalDialog.FindViewById(Resource.Id.txtDegrees)).Text = $"{WeatherData.Wind.Deg}";
            ModalDialog.Show();

            ((Button)ModalDialog.FindViewById(Resource.Id.btnOk)).Click += delegate
            {
                DismissLightboxDialog();
            };
        }


        string ConvertFromEpoch(int epoch) => new DateTime(1970,1,1,0,0,0,DateTimeKind.Utc).AddSeconds(epoch).ToShortDateString();

        public void DismissLightboxDialog()
        {
            if (ModalDialog != null)
                ModalDialog.Dismiss();

            ModalDialog = null;
            Service.GetService<WeatherViewModel>().ShowData = false;
        }
    }
}
