using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Graphics;

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
    [NotifyPropertyChangedFor(nameof(ProgressRect))]
    private double progress;

    public Rect ProgressRect => new Rect(0, 0.5, Progress, 12);

    [ObservableProperty]
    private bool isExpanded;

    [ObservableProperty]
    private double opacity = 1.0;

    [ObservableProperty]
    private bool isCurrentWeek;
}
