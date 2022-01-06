using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using WeatherApp.Interfaces;
using WeatherApp.Models;
using Xamarin.Essentials;

namespace WeatherApp.ViewModels
{
    public class WeatherViewModel : BaseViewModel
    {
        IRepository repository => (IRepository)Startup.ServiceProvider.GetService(typeof(IRepository));
        IWeatherService webservice => (IWeatherService)Startup.ServiceProvider.GetService(typeof(IWeatherService));
        IMessenger messenger => (IMessenger)Startup.ServiceProvider.GetService(typeof(IMessenger));

        public void Setup()
        {
            messenger.Register<BooleanMessage>(this, (m, t) =>
            {
                switch (t.Message)
                {
                    case "Location":
                        CanUseGeoloc = t.BoolValue;
                        break;
                }
            });

            var user = repository.GetData<UserSettings>();
            if (user != null)
            {
                UserSettings = user;
                City = user.CityName;
                if (user.IsUSState)
                    state = user.USState;
                Country = user.Country;
                UserOnStartup = user.AutoOnStart;
                CanUseGeoloc = user.CanUseGeolocation;
                Latitude = user.LastLat;
                Longitude = user.LastLng;

                PlaceName = $"{user.CityName}";
                PlaceName += user.IsUSState ? user.USState + ", " : ", ";
                PlaceName += user.Country;
                Geoloc = $"lat = {user.LastLat}, lng = {user.LastLng}";

                if (user.AutoOnStart)
                {
                    if (user.CanUseGeolocation)
                    {
                        GetDataForLocation.Execute(null);
                    }
                    else
                    {
                        GetDataForCity.Execute(null);
                    }
                }
            }
            else
                UserSettings = new UserSettings();
        }

        string? placeName;
        public string PlaceName
        {
            get => placeName;
            set => SetProperty(ref placeName, value);
        }

        string? geoloc;
        public string Geoloc
        {
            get => geoloc;
            set => SetProperty(ref geoloc, value);
        }

        UserSettings UserSettings { get; set; }
        public WeatherData WeatherData { get; set; }

        bool showData;
        public bool ShowData
        {
            get => showData;
            set => SetProperty(ref showData, value);
        }

        string city;
        public string City
        {
            get => city;
            set => SetProperty(ref city, value);
        }

        string state;
        public string State
        {
            get => state;
            set => SetProperty(ref state, value);
        }

        string country;
        public string Country
        {
            get => country;
            set => SetProperty(ref country, value);
        }

        bool canUseGeoloc;
        public bool CanUseGeoloc
        {
            get => canUseGeoloc;
            set
            {
                SetProperty(ref canUseGeoloc, value);
                UserSettings.CanUseGeolocation = value;
                repository.SaveData(UserSettings);
            }
        }

        Location location;
        public Location Location
        {
            get => location;
            set
            {
                SetProperty(ref location, value);
                Latitude = value.Latitude;
                Longitude = value.Longitude;
            }
        }

        public bool ResetData
        {
            set
            {
                UserSettings = new UserSettings();
                repository.SaveData(UserSettings);
            }
        }

        bool resetLocation;
        public bool ResetLocation
        {
            get => resetLocation;
            set
            {
                SetProperty(ref resetLocation, value);
                WeatherData = new WeatherData();
                SetProperty(ref resetLocation, false);
            }
        }

        double latitude;
        public double Latitude
        {
            get => latitude;
            set => SetProperty(ref latitude, value);
        }

        double longitude;
        public double Longitude
        {
            get => longitude;
            set => SetProperty(ref longitude, value);
        }

        bool getOnStart;
        public bool GetOnStart
        {
            get => getOnStart;
            set
            {
                SetProperty(ref getOnStart, value);
                UserSettings.AutoOnStart = value;
                repository.SaveData(UserSettings);
            }
        }

        bool userOnStartup;
        public bool UserOnStartup
        {
            get => userOnStartup;
            set => SetProperty(ref userOnStartup, value);
        }

        void DisplayPlace()
        {
            PlaceName = $"{WeatherData.Name}";
            PlaceName += !string.IsNullOrEmpty(state) ? state + ", " : ", ";
            PlaceName += WeatherData.Sys.Country;
            Geoloc = $"lat = {WeatherData.Coord.Lat}, lng = {WeatherData.Coord.Lon}";
        }

        void SaveSettings()
        {
            if (City == UserSettings.CityName)
            {
                UserSettings.AutoOnStart = UserOnStartup;
                UserSettings.CityName = City;
                UserSettings.Country = Country;
                if (!string.IsNullOrEmpty(state))
                {
                    UserSettings.IsUSState = true;
                    UserSettings.USState = state;
                }
                UserSettings.CanUseGeolocation = CanUseGeoloc;
                UserSettings.LastLat = Latitude;
                UserSettings.LastLng = Longitude;
                repository.SaveData(UserSettings);
            }
        }

        RelayCommand? getDataForCity;
        public RelayCommand GetDataForCity => getDataForCity ?? new RelayCommand(async () =>
        {
            var state = string.IsNullOrEmpty(State) ? "" : State;

            if (!string.IsNullOrEmpty(City) && !string.IsNullOrEmpty(Country))
            {
                IsBusy = true;
                messenger.Send(new BooleanMessage { BoolValue = IsBusy, Message = "IsBusy" });

                var pt = City.LastIndexOf(' ');
                if (pt != -1)
                    City = City.Substring(0, pt);
                pt = Country.LastIndexOf(' ');
                if (pt != -1)
                    Country= Country.Substring(0,pt);

                var data = await webservice.GetWeatherForCity(City, Country, state);
                {
                    if (data != null)
                    {
                        WeatherData = data;
                        DisplayPlace();
                        ShowData = true;
                        messenger.Send(new BooleanMessage { BoolValue = ShowData, Message = "HasData" });
                        if (UserOnStartup)
                        {
                            SaveSettings();
                        }
                    }
                    else
                        messenger.Send(new StringMessage { Message = "No data returned for this location", Sender = "CityData" });
                }
                IsBusy = false;
                messenger.Send(new BooleanMessage { BoolValue = IsBusy, Message = "IsBusy" });
            }
            else
                messenger.Send(new BooleanMessage { BoolValue = true, Message = "Error" });
        });

        RelayCommand? getDataForLocation;
        public RelayCommand GetDataForLocation => getDataForLocation ?? new RelayCommand(async () =>
        {
            IsBusy = true;
            messenger.Send(new BooleanMessage { BoolValue = IsBusy, Message = "IsBusy" });
            var data = await webservice.GetWeatherForLocation(Longitude, Latitude);
            if (data != null)
            {
                WeatherData = data;
                DisplayPlace();
                ShowData = true;
                messenger.Send(new BooleanMessage { BoolValue = ShowData, Message = "HasData" });
                if (UserOnStartup)
                {
                    SaveSettings();
                }
            }
            else
                messenger.Send(new StringMessage { Message = "No data returned for this geolocation", Sender = "CityData" });
            IsBusy = false;
            messenger.Send(new BooleanMessage { BoolValue = IsBusy, Message = "IsBusy" });
        });
    }
}
