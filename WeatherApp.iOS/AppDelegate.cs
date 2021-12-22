﻿using CommunityToolkit.Mvvm.Messaging;
using Foundation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using UIKit;
using WeatherApp.Models;
using WeatherApp.ViewModels;
using Xamarin.Essentials;

namespace WeatherApp.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register ("AppDelegate")]
    public class AppDelegate : UIResponder, IUIApplicationDelegate {
    
        [Export("window")]
        public UIWindow Window { get; set; }
        public static IServiceProvider Service { get; set; }
        IMessenger messenger => (IMessenger)Startup.ServiceProvider.GetService(typeof(IMessenger));

        public static AppDelegate Self { get; private set; }

        [Export ("application:didFinishLaunchingWithOptions:")]
        public bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
        {
            AppDelegate.Self = this;

            new SQLiteConnectionFactory().GetConnection();
            Service = WeatherApp.Startup.Init ();

            var connnect = (Connectivity.NetworkAccess == NetworkAccess.Internet) || (Connectivity.NetworkAccess == NetworkAccess.Local);
            Service.GetService<BaseViewModel>().IsConnected = connnect;
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;

            GetLocation();

            return true;
        }

        void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            Service.GetService<BaseViewModel>().IsConnected = Convert.ToBoolean(e.NetworkAccess);
        }

        void GetLocation()
        {
            var loc = Service.GetService<WeatherViewModel>();
            Task.Run(() =>
                BeginInvokeOnMainThread(async () =>
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
                                }
                            }
                            else
                                messenger.Send<BooleanMessage>(new BooleanMessage { BoolValue = false, Message = "Location" });
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
#if DEBUG
                        Console.WriteLine(ex.ToString());
#endif
                    }
                }));
        }

        // UISceneSession Lifecycle

        [Export ("application:configurationForConnectingSceneSession:options:")]
        public UISceneConfiguration GetConfiguration (UIApplication application, UISceneSession connectingSceneSession, UISceneConnectionOptions options)
        {
            // Called when a new scene session is being created.
            // Use this method to select a configuration to create the new scene with.
            return UISceneConfiguration.Create ("Default Configuration", connectingSceneSession.Role);
        }

        [Export ("application:didDiscardSceneSessions:")]
        public void DidDiscardSceneSessions (UIApplication application, NSSet<UISceneSession> sceneSessions)
        {
            // Called when the user discards a scene session.
            // If any sessions were discarded while the application was not running, this will be called shortly after `FinishedLaunching`.
            // Use this method to release any resources that were specific to the discarded scenes, as they will not return.
        }
    }
}

