using Weak.ViewModels;

namespace Weak.Views;

public partial class UnifiedCreationPage : ContentPage
{
    public UnifiedCreationPage(UnifiedCreationViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
