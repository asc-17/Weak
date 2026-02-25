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
    private readonly SettingsService _settingsService;

    [ObservableProperty]
    private string overviewText = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(UserInitial))]
    private string userName = string.Empty;

    public string UserInitial => string.IsNullOrWhiteSpace(UserName) ? "?" : UserName[0].ToString().ToUpper();

    public ObservableCollection<WeekCard> WeekCards { get; } = new();

    public HomeViewModel(WeekComputationService weekComputation, TaskRepository taskRepository, SettingsService settingsService)
    {
        _weekComputation = weekComputation;
        _taskRepository = taskRepository;
        _settingsService = settingsService;
    }

    public async Task InitializeAsync()
    {
        var settings = await _settingsService.GetSettingsAsync();
        UserName = settings.Name ?? string.Empty;
        await LoadWeekDataAsync();
    }

    public async Task LoadWeekDataAsync()
    {
        WeekCards.Clear();

        var thisWeekStart = await _weekComputation.GetWeekStartAsync(DateTime.Today);

        for (int weekOffset = 0; weekOffset < 3; weekOffset++)
        {
            var weekStart = thisWeekStart.AddDays(weekOffset * 7);
            var weekData = await _weekComputation.ComputeWeekDataAsync(weekStart);

            var weekCard = new WeekCard
            {
                Title = weekOffset == 0 ? "This Week" : weekOffset == 1 ? "Next Week" : "Week After",
                DateRange = $"{weekData.StartDate:MMM dd} - {weekData.EndDate:MMM dd}",
                LoadScore = weekData.LoadScore,
                WeightedProgress = weekData.WeightedProgress,
                Intensity = weekData.Intensity,
                Opacity = weekOffset == 0 ? 1.0 : weekOffset == 1 ? 0.6 : 0.5,
                IsCurrentWeek = weekOffset == 0,
                StartDate = weekData.StartDate,
                EndDate = weekData.EndDate
            };

            WeekCards.Add(weekCard);

            if (weekOffset == 0)
            {
                OverviewText = _weekComputation.GetRandomSummary(weekData.LoadScore, weekData.WeightedProgress);
            }
        }
    }

    [RelayCommand]
    private void ToggleExpand(WeekCard card)
    {
        card.IsExpanded = !card.IsExpanded;
    }

    [RelayCommand]
    private async Task NavigateToSettings()
    {
        await Shell.Current.GoToAsync("//SettingsView");
    }
}

