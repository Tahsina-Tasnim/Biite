using Biite.Pages;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace Biite;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit() //Use when you figure out why following the steps in Microsoft webpage did not work with correct package
            .ConfigureFonts(fonts =>
           
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .UseMauiMaps(); 

#if DEBUG
		builder.Logging.AddDebug();
#endif
       

        return builder.Build();
    }
}
