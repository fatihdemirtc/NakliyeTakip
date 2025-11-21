using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NakliyeTakip.MAUI.Options;
using NakliyeTakip.MAUI.Pages;
using NakliyeTakip.MAUI.Services;
using NakliyeTakip.MAUI.Services.Auth;
using NakliyeTakip.MAUI.Services.Refit;
using Refit;
using System.Reflection;

namespace NakliyeTakip.MAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("NakliyeTakip.MAUI.appsettings.json");
            if (stream != null)
            {
                var config = new ConfigurationBuilder().AddJsonStream(stream).Build();
                builder.Configuration.AddConfiguration(config);
            }
            builder.Services.Configure<GatewayOption>(builder.Configuration.GetSection(GatewayOption.SettingKey));

            // Authentication Service'i kaydet
            builder.Services.AddSingleton<IAuthenticationService, KeycloakAuthenticationService>();
            
            // HTTP Client için DelegatingHandler ekle
            builder.Services.AddTransient<AuthenticationDelegatingHandler>();
            builder.Services.AddSingleton<LocationService>();


            // Pages
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<MainPage>();

            builder.Services.AddRefitClient<ILocationRefitService>().ConfigureHttpClient(configure =>
            {
                var gatewayOption = builder.Configuration.GetSection(nameof(GatewayOption)).Get<GatewayOption>();
                configure.BaseAddress = new Uri(gatewayOption!.BaseAddress);
            }).AddHttpMessageHandler<AuthenticationDelegatingHandler>();

#if DEBUG
            builder.Logging.AddDebug();
#endif
           
            var app = builder.Build();

            return app;
        }
    }
}
