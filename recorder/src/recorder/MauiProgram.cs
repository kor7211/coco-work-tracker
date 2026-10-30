using Microsoft.Extensions.Logging;
using Recorder.Views;
using Recorder.Services;
using Recorder.Core.Models;
using Supabase;

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
                    fonts.AddFont("NunitoSans-VariableFont.ttf", "NunitoSans");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<AppState>();
            builder.Services.AddSingleton<AppShell>();
            builder.Services.AddSingleton<SupabaseSessionStorage>();
            builder.Services.AddSingleton(provider =>
            {
                var sessionStorage =
                    provider.GetRequiredService<SupabaseSessionStorage>();

                var options = new SupabaseOptions
                {
                    AutoRefreshToken = true,
                    SessionHandler = sessionStorage
                };

                return SupabaseClientGenerator.GetClient(options);
            });

            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<ListRecordPage>();
            builder.Services.AddTransient<ListTypePage>();

            return builder.Build();
        }
    }
}
