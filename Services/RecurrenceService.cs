using Weak.Models;

namespace Weak.Services;

public class RecurrenceService
{
    public Task<List<TaskItem>> GenerateInstancesAsync(TaskItem rootTask, int count)
    {
        var instances = new List<TaskItem>();
        for (int i = 1; i <= count; i++)
        {
            var instanceDate = AdvanceDate(rootTask.Deadline, rootTask.RecurrenceType, rootTask.RecurrenceInterval, i);
            instances.Add(new TaskItem
            {
                Title = rootTask.Title,
                Subject = rootTask.Subject,
                Category = rootTask.Category,
                Effort = rootTask.Effort,
                CompletionPercent = 0,
                Source = rootTask.Source,
                SubjectColor = rootTask.SubjectColor,
                Deadline = instanceDate,
                RecurrenceType = rootTask.RecurrenceType,
                RecurrenceInterval = rootTask.RecurrenceInterval,
                IsDayOnly = rootTask.IsDayOnly,
                RecurrenceParentId = rootTask.Id,
                CreatedAt = DateTime.UtcNow
            });
        }
        return Task.FromResult(instances);
    }

    private static DateTime AdvanceDate(DateTime start, string type, int interval, int step) =>
        type switch
        {
            "daily"   => start.AddDays(step),
            "weekly"  => start.AddDays(7 * step),
            "monthly" => start.AddMonths(step),
            "custom"  => start.AddDays(interval * step),
            _         => start.AddDays(step)
        };

    public static int GetInstanceCount(TaskItem task) => task.RecurrenceType switch
    {
        "daily"   => 30,
        "weekly"  => 12,
        "monthly" => 6,
        "custom"  => 30,
        _         => 0
    };
}
