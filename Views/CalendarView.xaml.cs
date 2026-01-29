using Weak.ViewModels;

namespace Weak.Views;

public partial class CalendarView : ContentPage
{
	public CalendarView(CalendarViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
