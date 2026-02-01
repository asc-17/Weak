using Weak.ViewModels;

namespace Weak.Views;

public partial class TasksView : ContentPage
{
	private readonly TasksViewModel _viewModel;

	public TasksView(TasksViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await _viewModel.InitializeAsync();
	}
}
