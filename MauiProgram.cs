using Microsoft.Extensions.Logging;
using Weak.Views;
using Weak.ViewModels;

namespace Weak
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
                    fonts.AddFont("Inter-Regular.ttf", "InterRegular");
                    fonts.AddFont("Inter-Medium.ttf", "InterMedium");
                    fonts.AddFont("Inter-SemiBold.ttf", "InterSemiBold");
                    fonts.AddFont("Inter-Bold.ttf", "InterBold");
                });

        builder.Services.AddSingleton<HomeViewModel>();
        builder.Services.AddSingleton<HomeView>();

        builder.Services.AddSingleton<TasksViewModel>();
        builder.Services.AddSingleton<TasksView>();

        builder.Services.AddSingleton<CalendarViewModel>();
        builder.Services.AddSingleton<CalendarView>();

        builder.Services.AddSingleton<SettingsViewModel>();
        builder.Services.AddSingleton<SettingsView>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
