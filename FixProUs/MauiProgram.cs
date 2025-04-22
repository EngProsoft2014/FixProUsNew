using CommunityToolkit.Maui;
using Controls.UserDialogs.Maui;
using FFImageLoading.Maui;
using Microsoft.Extensions.Logging;
using Mopups.Hosting;
using Syncfusion.Maui.Core.Hosting;

namespace FixProUs
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureMopups()
                .UseUserDialogs()
                .ConfigureSyncfusionCore()
                .UseFFImageLoading()
                .UseMauiMaps()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Font Awesome 6 Brands-Regular-400.otf", "FontIconBrand");
                    fonts.AddFont("Font Awesome 6 Free-Regular-400.otf", "FontIcon");
                    fonts.AddFont("Font Awesome 6 Free-Solid-900.otf", "FontIconSolid");
                    fonts.AddFont("Amiri-Bold.ttf", "AmiriBold");
                    fonts.AddFont("Amiri-BoldItalic.ttf", "AmiriBoldItalic");
                    fonts.AddFont("Amiri-Regular.ttf", "AmiriRegular");
                    fonts.AddFont("Almarai-Bold.ttf", "AlmaraiBold");
                    fonts.AddFont("Almarai-ExtraBold.ttf", "AlmaraiExtraBold");
                    fonts.AddFont("Almarai-Light.ttf", "AlmaraiLight");
                    fonts.AddFont("Almarai-Regular.ttf", "AlmaraiRegular");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddDependencies();

            DependencyInjection.ControlsBackground();

            return builder.Build();
        }
    }
}
