using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Weak.Models;

public partial class CalendarDay : ObservableObject
{
    [ObservableProperty]
    private DateTime date;

    [ObservableProperty]
    private int dayNumber;

    [ObservableProperty]
    private bool isCurrentMonth;

    [ObservableProperty]
    private bool isToday;

    [ObservableProperty]
    private bool isSelected;

    [ObservableProperty]
    private int taskCount;

    [ObservableProperty]
    private bool hasHighPriorityTask;

    [ObservableProperty]
    private string taskIndicatorColor = "#104EB0";

    [ObservableProperty]
    private double dailyEffort;

    [ObservableProperty]
    private string effortBarColor = "Transparent";

    [ObservableProperty]
    private bool hasEffort;

    public ObservableCollection<string> TaskIndicatorColors { get; } = new();

    public CalendarDay(DateTime date, bool isCurrentMonth)
    {
        Date = date;
        DayNumber = date.Day;
        IsCurrentMonth = isCurrentMonth;
        IsToday = date.Date == DateTime.Today;
    }
}
