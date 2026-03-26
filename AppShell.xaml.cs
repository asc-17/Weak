using Weak.Views;

namespace Weak
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register routes for navigation
            Routing.RegisterRoute("edittask", typeof(EditTaskPage));
            Routing.RegisterRoute("create", typeof(UnifiedCreationPage));
            Routing.RegisterRoute("listpage", typeof(ListPage));
            Routing.RegisterRoute("yearlyoverview", typeof(YearlyOverviewPage));
        }
    }
}
