using Weak.Views;

namespace Weak
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register routes for navigation
            Routing.RegisterRoute("createtask", typeof(CreateTaskPage));
            Routing.RegisterRoute("edittask", typeof(EditTaskPage));
            Routing.RegisterRoute("createlist", typeof(CreateListPage));
            Routing.RegisterRoute("yearlyoverview", typeof(YearlyOverviewPage));
        }
    }
}
