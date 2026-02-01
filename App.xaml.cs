using Weak.Services;

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
            return new Window(new AppShell());
        }

        protected override async void OnStart()
        {
            base.OnStart();
            
            // Initialize database asynchronously
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