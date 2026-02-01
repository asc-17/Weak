using Weak.ViewModels;

namespace Weak.Views;

public partial class CreateTaskPage : ContentPage
{
    public CreateTaskPage(CreateTaskViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
