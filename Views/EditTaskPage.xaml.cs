using Weak.ViewModels;

namespace Weak.Views;

public partial class EditTaskPage : ContentPage
{
    public EditTaskPage(EditTaskViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
