using CommunityToolkit.Maui;
using H.Necessaire.Runtime.MAUI;
using Microsoft.Extensions.Logging;

namespace H.Necessaire.Runtime.MAUIAppSample
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            MauiAppBuilder builder = MauiApp.CreateBuilder();
#pragma warning disable MCT001 // `.UseMauiCommunityToolkit()` Not Found on MauiAppBuilder
            builder
                .UseMauiApp<App>(sp => new App().InitializeHNecessaireApp())
                .WithHNecessaire()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddHNecessaireFonts();
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
#pragma warning restore MCT001 // `.UseMauiCommunityToolkit()` Not Found on MauiAppBuilder

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
