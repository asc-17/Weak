using Weak.ViewModels;

namespace Weak.Views;

public partial class YearlyOverviewPage : ContentPage
{
    private readonly YearlyOverviewViewModel _viewModel;

    public YearlyOverviewPage(YearlyOverviewViewModel viewModel)
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
