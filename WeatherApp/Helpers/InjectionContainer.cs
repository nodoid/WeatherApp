using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using WeatherApp.Database;
using WeatherApp.Interfaces;
using WeatherApp.Services;
using WeatherApp.ViewModels;

namespace WeatherApp.Helpers
{
    public static class  InjectionContainer
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            var i = new ServiceCollection();

            i.AddSingleton<IWebservice, Webservice>().
                AddSingleton<IMessenger>(WeakReferenceMessenger.Default).
                AddSingleton<IRepository, SqLiteRepository>().
                AddSingleton<ISqLiteConnectionFactory, Connection>();

            services = i;

            return services;
        }

        public static IServiceCollection ConfigureViewModels(this IServiceCollection services)
        {
            services.AddTransient<BaseViewModel>().
                AddTransient<WeatherViewModel>();

            return services;
        }

    }
}
