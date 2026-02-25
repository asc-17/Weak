using Microsoft.Extensions.Logging;
using Weak.Views;
using Weak.ViewModels;
using Weak.Services;

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

        // Services
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<TaskRepository>();
        builder.Services.AddSingleton<SettingsService>();
        builder.Services.AddSingleton<WeekComputationService>();
        builder.Services.AddSingleton<CalendarImportService>();
        
#if ANDROID
        builder.Services.AddSingleton<INotificationService, Platforms.Android.Services.NotificationService>();
#else
        builder.Services.AddSingleton<INotificationService>(sp => null!);
#endif

        // ViewModels and Views
        builder.Services.AddSingleton<HomeViewModel>();
        builder.Services.AddSingleton<HomeView>();

        builder.Services.AddSingleton<TasksViewModel>();
        builder.Services.AddSingleton<TasksView>();

        builder.Services.AddTransient<CreateTaskViewModel>();
        builder.Services.AddTransient<CreateTaskPage>();

        builder.Services.AddTransient<EditTaskViewModel>();
        builder.Services.AddTransient<EditTaskPage>();

        builder.Services.AddSingleton<CalendarViewModel>();
        builder.Services.AddSingleton<CalendarView>();

        builder.Services.AddSingleton<SettingsViewModel>();
        builder.Services.AddSingleton<SettingsView>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            return app;
        }
    }
}
