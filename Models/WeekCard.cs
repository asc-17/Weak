using CommunityToolkit.Mvvm.ComponentModel;

namespace Weak.Models;

public partial class WeekCard : ObservableObject
{
    [ObservableProperty]
    private string title;

    [ObservableProperty]
    private string dateRange;

    [ObservableProperty]
    private double score;

    [ObservableProperty]
    private string intensity;

    [ObservableProperty]
    private double progress;

    [ObservableProperty]
    private bool isExpanded;
}
