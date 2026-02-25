using Weak.Models;

namespace Weak.Services;

public class WeekComputationService
{
    private readonly DatabaseService _database;
    private readonly SettingsService _settingsService;

    public WeekComputationService(DatabaseService database, SettingsService settingsService)
    {
        _database = database;
        _settingsService = settingsService;
    }

    [Obsolete("Use GetWeekStartAsync to respect the user's week start preference.")]
    public static DateTime GetWeekStart(DateTime date)
    {
        var daysSinceSunday = (int)date.DayOfWeek;
        return date.Date.AddDays(-daysSinceSunday);
    }

    public static DateTime GetWeekEnd(DateTime date)
    {
        return GetWeekStart(date).AddDays(6);
    }

    public async Task<DateTime> GetWeekStartAsync(DateTime date)
    {
        var settings = await _settingsService.GetSettingsAsync();
        var offset = settings.WeekStartDay == "monday" ? 1 : 0;
        var daysSinceStart = ((int)date.DayOfWeek - offset + 7) % 7;
        return date.Date.AddDays(-daysSinceStart);
    }

    public async Task<WeekData> ComputeWeekDataAsync(DateTime weekStart)
    {
        var weekEnd = weekStart.AddDays(6);

        var weekTasks = await _database.GetTasksByWeekAsync(weekStart, weekEnd);

        var overdueTasks = new List<TaskItem>();
#pragma warning disable CS0618
        if (weekStart.Date == GetWeekStart(DateTime.Today))
#pragma warning restore CS0618
        {
            overdueTasks = await _database.GetOverdueTasksAsync();
        }

        var allTasks = weekTasks.Concat(overdueTasks).ToList();

        var loadScore = CalculateLoadScore(allTasks);
        var weightedProgress = CalculateWeightedProgress(allTasks);
        var intensity = GetIntensity(loadScore);

        return new WeekData
        {
            StartDate = weekStart,
            EndDate = weekEnd,
            LoadScore = loadScore,
            WeightedProgress = weightedProgress,
            Intensity = intensity,
            TaskCount = allTasks.Count
        };
    }

    public double CalculateLoadScore(List<TaskItem> tasks)
    {
        if (!tasks.Any())
            return 0.0;

        var totalEffort = tasks.Sum(t => t.Effort);
        if (totalEffort == 0)
            return 0.0;

        var weightedRemaining = tasks.Sum(t => t.Effort * (1.0 - t.CompletionPercent / 100.0));
        return (weightedRemaining / totalEffort) * 10.0;
    }

    public double CalculateWeightedProgress(List<TaskItem> tasks)
    {
        if (!tasks.Any())
            return 0.0;

        var totalEffort = tasks.Sum(t => t.Effort);
        if (totalEffort == 0)
            return 0.0;

        var weightedCompletion = tasks.Sum(t => t.Effort * (t.CompletionPercent / 100.0));
        return weightedCompletion / totalEffort;
    }

    public string GetIntensity(double loadScore)
    {
        if (loadScore <= 3.0)
            return "Low";
        else if (loadScore <= 6.0)
            return "Moderate";
        else if (loadScore <= 8.5)
            return "High";
        else
            return "Critical";
    }

    public string GetRandomSummary(double loadScore, double weightedProgress)
    {
        var summaries = new List<string>();

        if (loadScore >= 8.0)
        {
            if (weightedProgress >= 0.7)
                summaries.AddRange(new[]
                {
                    "Heavy week, but you're crushing it.",
                    "High load, strong progress. Keep going.",
                    "Demanding week. You're handling it well."
                });
            else if (weightedProgress >= 0.4)
                summaries.AddRange(new[]
                {
                    "Your academic load is heavy this week. Prioritize key assignments.",
                    "Heavy week ahead. Focus on what matters most.",
                    "Tough week. Stay focused on priorities."
                });
            else
                summaries.AddRange(new[]
                {
                    "Heavy week with slow progress. Time to catch up.",
                    "Demanding week. Consider breaking tasks down.",
                    "High load, low momentum. Start with one task."
                });
        }
        else if (loadScore >= 4.0)
        {
            if (weightedProgress >= 0.7)
                summaries.AddRange(new[]
                {
                    "Balanced week, great progress.",
                    "Next week looks manageable.",
                    "Steady load. You're on track."
                });
            else if (weightedProgress >= 0.4)
                summaries.AddRange(new[]
                {
                    "Moderate workload. Keep momentum going.",
                    "Manageable week. Stay consistent.",
                    "Balanced week. Maintain your rhythm."
                });
            else
                summaries.AddRange(new[]
                {
                    "Moderate week. Time to build momentum.",
                    "Balanced load but progress is lagging.",
                    "Average week. Push forward on tasks."
                });
        }
        else
        {
            if (weightedProgress >= 0.7)
                summaries.AddRange(new[]
                {
                    "Light week, excellent progress.",
                    "Smooth sailing. Great work.",
                    "Easy week. You're ahead of the game."
                });
            else
                summaries.AddRange(new[]
                {
                    "You have room to breathe this week.",
                    "Light week ahead. Good time to plan.",
                    "Calm week. Use it wisely."
                });
        }

        return summaries[Random.Shared.Next(summaries.Count)];
    }
}

public class WeekData
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double LoadScore { get; set; }
    public double WeightedProgress { get; set; }
    public string Intensity { get; set; } = string.Empty;
    public int TaskCount { get; set; }
}
