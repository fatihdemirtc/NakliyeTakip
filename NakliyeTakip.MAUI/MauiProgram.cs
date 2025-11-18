using Microsoft.Extensions.Logging;
using NakliyeTakip.MAUI.Services;
using NakliyeTakip.MAUI.Pages;

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

            // Authentication Service'i kaydet
            builder.Services.AddSingleton<IAuthenticationService, KeycloakAuthenticationService>();
            
            // HTTP Client için DelegatingHandler ekle
            builder.Services.AddTransient<AuthenticationDelegatingHandler>();
            
            // Gateway API için HttpClient
            builder.Services.AddHttpClient("GatewayAPI", client =>
            {
                // TODO: Gateway URL'nizi buraya yazın
                client.BaseAddress = new Uri("http://localhost:5000");
            })
            .AddHttpMessageHandler<AuthenticationDelegatingHandler>();

            // Pages
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<MainPage>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
