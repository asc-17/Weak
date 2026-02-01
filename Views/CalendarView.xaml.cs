using Weak.ViewModels;

namespace Weak.Views;

public partial class CalendarView : ContentPage
{
	private readonly CalendarViewModel _viewModel;

	public CalendarView(CalendarViewModel viewModel)
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
