using Weak.ViewModels;

namespace Weak.Views;

public partial class SettingsView : ContentPage
{
	private readonly SettingsViewModel _viewModel;

	public SettingsView(SettingsViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = viewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await _viewModel.InitializeAsync();
	}
}
