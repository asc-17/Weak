using Weak.ViewModels;

namespace Weak.Views;

public partial class HomeView : ContentPage
{
	private readonly HomeViewModel _viewModel;

	public HomeView(HomeViewModel viewModel)
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
