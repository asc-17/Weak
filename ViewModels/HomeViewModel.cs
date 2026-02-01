using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Weak.Models;
using Weak.Services;

namespace Weak.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly WeekComputationService _weekComputation;
    private readonly TaskRepository _taskRepository;

    [ObservableProperty]
    private string overviewText = string.Empty;

    public ObservableCollection<WeekCard> WeekCards { get; } = new();

    public HomeViewModel(WeekComputationService weekComputation, TaskRepository taskRepository)
    {
        _weekComputation = weekComputation;
        _taskRepository = taskRepository;
    }

    public async Task InitializeAsync()
    {
        await LoadWeekDataAsync();
    }

    public async Task LoadWeekDataAsync()
    {
        WeekCards.Clear();

        var thisWeekStart = WeekComputationService.GetWeekStart(DateTime.Today);

        for (int weekOffset = 0; weekOffset < 3; weekOffset++)
        {
            var weekStart = thisWeekStart.AddDays(weekOffset * 7);
            var weekData = await _weekComputation.ComputeWeekDataAsync(weekStart);

            var weekCard = new WeekCard
            {
                Title = weekOffset == 0 ? "This Week" : weekOffset == 1 ? "Next Week" : "Week After",
                DateRange = $"{weekData.StartDate:MMM dd} - {weekData.EndDate:MMM dd}",
                Score = weekData.Progress,
                Load = weekData.TotalLoad,
                Intensity = weekData.Intensity,
                Progress = weekData.Progress / 10.0,
                Opacity = weekOffset == 0 ? 1.0 : weekOffset == 1 ? 0.6 : 0.5,
                IsCurrentWeek = weekOffset == 0,
                StartDate = weekData.StartDate,
                EndDate = weekData.EndDate
            };

            WeekCards.Add(weekCard);

            if (weekOffset == 0)
            {
                OverviewText = _weekComputation.GetRandomSummary(weekData.TotalLoad, weekData.Progress);
            }
        }
    }

    [RelayCommand]
    private void ToggleExpand(WeekCard card)
    {
        card.IsExpanded = !card.IsExpanded;
    }
}
