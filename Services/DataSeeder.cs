using Weak.Models;

namespace Weak.Services;

public class DataSeeder
{
    private readonly TaskRepository _taskRepository;

    public DataSeeder(TaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task SeedSampleDataAsync()
    {
        var existingTasks = await _taskRepository.GetAllTasksAsync();
        if (existingTasks.Any())
            return;

        var thisWeekStart = WeekComputationService.GetWeekStart(DateTime.Today);
        
        var sampleTasks = new List<TaskItem>
        {
            new TaskItem
            {
                Title = "Read Chapter 4: Macroeconomics",
                Deadline = DateTime.Today,
                Effort = 6,
                CompletionPercent = 0,
                SubjectColor = "#ef4444",
                Source = TaskSource.Manual,
                Subject = "Economics"
            },
            new TaskItem
            {
                Title = "Submit Calculus Problem Set",
                Deadline = DateTime.Today.AddDays(-1),
                Effort = 8,
                CompletionPercent = 100,
                SubjectColor = "#3b82f6",
                Source = TaskSource.Manual,
                Subject = "Calculus II"
            },
            new TaskItem
            {
                Title = "Draft History Essay",
                Deadline = thisWeekStart.AddDays(5),
                Effort = 10,
                CompletionPercent = 35,
                SubjectColor = "#eab308",
                Source = TaskSource.Manual,
                Subject = "History"
            },
            new TaskItem
            {
                Title = "Lab Report: Physics",
                Deadline = thisWeekStart.AddDays(7).AddDays(1),
                Effort = 7,
                CompletionPercent = 0,
                SubjectColor = "#8b5cf6",
                Source = TaskSource.Manual,
                Subject = "Physics"
            },
            new TaskItem
            {
                Title = "Group Project: Marketing Plan",
                Deadline = thisWeekStart.AddDays(14),
                Effort = 9,
                CompletionPercent = 0,
                SubjectColor = "#10b981",
                Source = TaskSource.Manual,
                Subject = "Business"
            },
            new TaskItem
            {
                Title = "Chemistry Problem Set",
                Deadline = thisWeekStart.AddDays(3),
                Effort = 5,
                CompletionPercent = 60,
                SubjectColor = "#f97316",
                Source = TaskSource.Manual,
                Subject = "Chemistry"
            }
        };

        foreach (var task in sampleTasks)
        {
            await _taskRepository.SaveTaskAsync(task);
        }
    }
}
