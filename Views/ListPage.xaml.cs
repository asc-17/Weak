using Weak.ViewModels;

namespace Weak.Views;

public partial class ListPage : ContentPage
{
    private readonly ListViewModel _viewModel;

    public ListPage(ListViewModel viewModel)
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
