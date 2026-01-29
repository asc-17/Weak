using CommunityToolkit.Mvvm.ComponentModel;

namespace Weak.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty]
    private bool notificationsEnabled = false;

    [ObservableProperty]
    private bool calendarSyncEnabled = true;

    public SettingsViewModel()
    {
    }
}
