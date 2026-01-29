using Weak.ViewModels;

namespace Weak.Views;

public partial class TasksView : ContentPage
{
	public TasksView(TasksViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
