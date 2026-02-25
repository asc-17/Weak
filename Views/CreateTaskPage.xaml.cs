using Weak.ViewModels;

namespace Weak.Views;

public partial class CreateTaskPage : ContentPage
{
    private readonly CreateTaskViewModel _viewModel;

    public CreateTaskPage(CreateTaskViewModel viewModel)
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
