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
    [NotifyPropertyChangedFor(nameof(LoadColor), nameof(LoadProgress))]
    private double loadScore;

    [ObservableProperty]
    private string intensity = string.Empty;

    [ObservableProperty]
    private double weightedProgress;

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
            if (LoadScore <= 3.0)
                return Color.FromArgb("#22c55e");
            else if (LoadScore <= 6.0)
                return Color.FromArgb("#eab308");
            else if (LoadScore <= 8.5)
                return Color.FromArgb("#f97316");
            else
                return Color.FromArgb("#ef4444");
        }
    }

    public double LoadProgress => Math.Min(LoadScore / 10.0, 1.0);

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

