using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Weak.Models;
using Weak.Services;

namespace Weak.ViewModels;

public partial class YearlyOverviewViewModel : ObservableObject
{
    private readonly WeekComputationService _weekComputation;

    [ObservableProperty]
    private int displayYear;

    public ObservableCollection<YearWeekCell> WeekCells { get; } = new();

    public YearlyOverviewViewModel(WeekComputationService weekComputation)
    {
        _weekComputation = weekComputation;
        DisplayYear = DateTime.Today.Year;
    }

    public async Task InitializeAsync()
    {
        await LoadYearDataAsync();
    }

    private async Task LoadYearDataAsync()
    {
        WeekCells.Clear();

        var jan1 = new DateTime(DisplayYear, 1, 1);
        var firstWeekStart = await _weekComputation.GetWeekStartAsync(jan1);

        // If the first week start is in the previous year, move to next week
        if (firstWeekStart.Year < DisplayYear)
            firstWeekStart = firstWeekStart.AddDays(7);

        var weekStart = firstWeekStart;
        int weekNum = 1;

        while (weekStart.Year == DisplayYear && weekNum <= 53)
        {
            var weekEnd = weekStart.AddDays(6);
            var weekData = await _weekComputation.ComputeWeekDataAsync(weekStart);

            WeekCells.Add(new YearWeekCell
            {
                WeekNumber = weekNum,
                WeekStart = weekStart,
                WeekEnd = weekEnd,
                LoadScore = weekData.LoadScore,
                Intensity = weekData.Intensity
            });

            weekStart = weekStart.AddDays(7);
            weekNum++;
        }
    }

    [RelayCommand]
    private async Task PreviousYear()
    {
        DisplayYear--;
        await LoadYearDataAsync();
    }

    [RelayCommand]
    private async Task NextYear()
    {
        DisplayYear++;
        await LoadYearDataAsync();
    }

    [RelayCommand]
    private async Task TapWeek(YearWeekCell cell)
    {
        if (cell == null) return;

        await Shell.Current.DisplayAlert(
            $"Week {cell.WeekNumber}",
            $"{cell.DateRange}\nLoad Score: {cell.LoadScore:F1} / 10\nIntensity: {cell.Intensity}",
            "OK");
    }
}
