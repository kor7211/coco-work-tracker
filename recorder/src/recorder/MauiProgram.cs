using Microsoft.Extensions.Logging;
using Recorder.Views;
using Recorder.Core.Models;

namespace Recorder
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

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<AppState>();

            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<ListRecordPage>();
            builder.Services.AddTransient<ListTypePage>();

            return builder.Build();
        }
    }
}
