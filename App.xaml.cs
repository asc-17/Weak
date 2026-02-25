using Weak.Services;
using Weak.Views.Onboarding;

namespace Weak
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var services = activationState?.Context?.Services;
            var settingsService = services?.GetService<SettingsService>();

            bool onboardingDone = false;
            if (settingsService != null)
            {
                onboardingDone = Task.Run(async () =>
                    await settingsService.IsOnboardingCompleteAsync()
                ).GetAwaiter().GetResult();
            }

            Page rootPage;
            if (onboardingDone || services == null)
                rootPage = new AppShell();
            else
                rootPage = (Page?)services.GetService<OnboardingPage>() ?? new AppShell();

            return new Window(rootPage);
        }

        protected override async void OnStart()
        {
            base.OnStart();

            var database = Handler?.MauiContext?.Services.GetService<DatabaseService>();
            if (database != null)
            {
                await database.InitializeAsync();

                var taskRepository = Handler?.MauiContext?.Services.GetService<TaskRepository>();
                if (taskRepository != null)
                {
                    var seeder = new DataSeeder(taskRepository);
                    await seeder.SeedSampleDataAsync();
                }
            }
        }
    }
}
