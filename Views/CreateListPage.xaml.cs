using Weak.ViewModels;

namespace Weak.Views;

public partial class CreateListPage : ContentPage
{
    public CreateListPage(CreateListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
