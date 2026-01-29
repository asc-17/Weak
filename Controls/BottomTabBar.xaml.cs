using System.Windows.Input;

namespace Weak.Controls;

public partial class BottomTabBar : Grid
{
    public static readonly BindableProperty SelectedTabProperty =
        BindableProperty.Create(nameof(SelectedTab), typeof(string), typeof(BottomTabBar), "Agenda", propertyChanged: OnSelectedTabChanged);

    public static readonly BindableProperty AgendaCommandProperty =
        BindableProperty.Create(nameof(AgendaCommand), typeof(ICommand), typeof(BottomTabBar));

    public static readonly BindableProperty TodoCommandProperty =
        BindableProperty.Create(nameof(TodoCommand), typeof(ICommand), typeof(BottomTabBar));

    public static readonly BindableProperty WorkloadCommandProperty =
        BindableProperty.Create(nameof(WorkloadCommand), typeof(ICommand), typeof(BottomTabBar));

    public static readonly BindableProperty SettingsCommandProperty =
        BindableProperty.Create(nameof(SettingsCommand), typeof(ICommand), typeof(BottomTabBar));

    // Color properties for active tab
    public static readonly BindableProperty AgendaBackgroundProperty =
        BindableProperty.Create(nameof(AgendaBackground), typeof(Color), typeof(BottomTabBar), Colors.Transparent);

    public static readonly BindableProperty AgendaTextColorProperty =
        BindableProperty.Create(nameof(AgendaTextColor), typeof(Color), typeof(BottomTabBar), Color.FromArgb("#94a3b8"));

    public BottomTabBar()
    {
        InitializeComponent();
        UpdateTabColors();
    }

    public string SelectedTab
    {
        get => (string)GetValue(SelectedTabProperty);
        set => SetValue(SelectedTabProperty, value);
    }

    public ICommand AgendaCommand
    {
        get => (ICommand)GetValue(AgendaCommandProperty);
        set => SetValue(AgendaCommandProperty, value);
    }

    public ICommand TodoCommand
    {
        get => (ICommand)GetValue(TodoCommandProperty);
        set => SetValue(TodoCommandProperty, value);
    }

    public ICommand WorkloadCommand
    {
        get => (ICommand)GetValue(WorkloadCommandProperty);
        set => SetValue(WorkloadCommandProperty, value);
    }

    public ICommand SettingsCommand
    {
        get => (ICommand)GetValue(SettingsCommandProperty);
        set => SetValue(SettingsCommandProperty, value);
    }

    public Color AgendaBackground
    {
        get => (Color)GetValue(AgendaBackgroundProperty);
        set => SetValue(AgendaBackgroundProperty, value);
    }

    public Color AgendaTextColor
    {
        get => (Color)GetValue(AgendaTextColorProperty);
        set => SetValue(AgendaTextColorProperty, value);
    }

    private static void OnSelectedTabChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is BottomTabBar tabBar)
        {
            tabBar.UpdateTabColors();
        }
    }

    private void UpdateTabColors()
    {
        // Reset all to inactive state first
        AgendaBackground = Colors.Transparent;
        AgendaTextColor = Color.FromArgb("#94a3b8");
        
        // Set colors based on selected tab
        switch (SelectedTab)
        {
            case "Agenda":
                AgendaBackground = Color.FromArgb("#ecf2ff");
                AgendaTextColor = Color.FromArgb("#135bec");
                break;
            case "To-Do":
            case "Workload":
            case "Settings":
                // For now, only Agenda is implemented
                // Will be extended when other tabs are active
                break;
        }
    }
}
