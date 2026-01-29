using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Weak.Models;

namespace Weak.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    [ObservableProperty]
    private string overviewText;

    public ObservableCollection<WeekCard> WeekCards { get; } = new();

    public HomeViewModel()
    {
        LoadMockData();
    }

    private void LoadMockData()
    {
        WeekCards.Clear();

        // This Week
        var thisWeek = new WeekCard
        {
            Title = "This Week",
            DateRange = GetDateRange(0),
            Score = 8.2,
            Intensity = "High Load",
            Progress = 0.82
        };

        // Next Week
        var nextWeek = new WeekCard
        {
            Title = "Next Week",
            DateRange = GetDateRange(1),
            Score = 4.5,
            Intensity = "Moderate",
            Progress = 0.45
        };

        // Week After
        var weekAfter = new WeekCard
        {
            Title = "Week After",
            DateRange = GetDateRange(2),
            Score = 1.2,
            Intensity = "Low",
            Progress = 0.12
        };

        WeekCards.Add(thisWeek);
        WeekCards.Add(nextWeek);
        WeekCards.Add(weekAfter);

        UpdateOverviewText(thisWeek.Score);
    }

    private string GetDateRange(int weekOffset)
    {
        var start = DateTime.Now.AddDays(weekOffset * 7);
        var end = start.AddDays(6);
        return $"{start:MMM dd} - {end:MMM dd}";
    }

    private void UpdateOverviewText(double currentScore)
    {
        if (currentScore > 7)
            OverviewText = "Your academic load is heavy this week. Prioritize key assignments.";
        else if (currentScore > 4)
            OverviewText = "Next week looks manageable.";
        else
            OverviewText = "You have room to breathe this week.";
    }

    [RelayCommand]
    private void ToggleExpand(WeekCard card)
    {
        card.IsExpanded = !card.IsExpanded;
    }
}
