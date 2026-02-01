using Weak.Models;

namespace Weak.Services;

public class WeekComputationService
{
    private readonly DatabaseService _database;

    public WeekComputationService(DatabaseService database)
    {
        _database = database;
    }

    public static DateTime GetWeekStart(DateTime date)
    {
        var daysSinceSunday = (int)date.DayOfWeek;
        return date.Date.AddDays(-daysSinceSunday);
    }

    public static DateTime GetWeekEnd(DateTime date)
    {
        return GetWeekStart(date).AddDays(6);
    }

    public async Task<WeekData> ComputeWeekDataAsync(DateTime weekStart)
    {
        var weekEnd = weekStart.AddDays(6);
        
        var weekTasks = await _database.GetTasksByWeekAsync(weekStart, weekEnd);
        
        var overdueTasks = new List<TaskItem>();
        if (weekStart.Date == GetWeekStart(DateTime.Today))
        {
            overdueTasks = await _database.GetOverdueTasksAsync();
        }
        
        var allTasks = weekTasks.Concat(overdueTasks).ToList();
        
        var totalLoad = CalculateTotalLoad(allTasks);
        var progress = CalculateProgress(allTasks);
        var intensity = GetIntensity(totalLoad);
        
        return new WeekData
        {
            StartDate = weekStart,
            EndDate = weekEnd,
            TotalLoad = totalLoad,
            Progress = progress,
            Intensity = intensity,
            TaskCount = allTasks.Count
        };
    }

    private double CalculateTotalLoad(List<TaskItem> tasks)
    {
        if (!tasks.Any())
            return 0;
        
        return tasks.Sum(t => t.Effort);
    }

    private double CalculateProgress(List<TaskItem> tasks)
    {
        if (!tasks.Any())
            return 0;

        var totalEffort = tasks.Sum(t => t.Effort);
        
        if (totalEffort == 0)
            return 0;

        var weightedCompletion = tasks.Sum(t => t.Effort * (t.CompletionPercent / 100.0));
        
        return (weightedCompletion / totalEffort) * 10.0;
    }

    private string GetIntensity(double load)
    {
        if (load < 20)
            return "Low";
        else if (load < 40)
            return "Moderate";
        else
            return "High Load";
    }

    public string GetRandomSummary(double load, double progress)
    {
        var summaries = new List<string>();

        if (load >= 40)
        {
            if (progress >= 7)
                summaries.AddRange(new[]
                {
                    "Heavy week, but you're crushing it.",
                    "High load, strong progress. Keep going.",
                    "Demanding week. You're handling it well."
                });
            else if (progress >= 4)
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
        else if (load >= 20)
        {
            if (progress >= 7)
                summaries.AddRange(new[]
                {
                    "Balanced week, great progress.",
                    "Next week looks manageable.",
                    "Steady load. You're on track."
                });
            else if (progress >= 4)
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
            if (progress >= 7)
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
    public double TotalLoad { get; set; }
    public double Progress { get; set; }
    public string Intensity { get; set; } = string.Empty;
    public int TaskCount { get; set; }
}
