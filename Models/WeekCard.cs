using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Graphics;

namespace Weak.Models;

public partial class WeekCard : ObservableObject
{
    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    private string dateRange = string.Empty;

    [ObservableProperty]
    private double score;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LoadColor), nameof(LoadProgress))]
    private double load;

    [ObservableProperty]
    private string intensity = string.Empty;

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

    public Color LoadColor
    {
        get
        {
            if (Load < 20)
                return Color.FromArgb("#22c55e");
            else if (Load < 40)
                return Color.FromArgb("#eab308");
            else
                return Color.FromArgb("#ef4444");
        }
    }

    // Normalize load to 0-1 range for ProgressBar
    // Assuming max load of 100 (10 tasks × effort of 10 each)
    public double LoadProgress => Math.Min(Load / 100.0, 1.0);

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
