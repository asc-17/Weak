using Weak.Models;

namespace Weak.Services;

public class TaskRepository
{
    private readonly DatabaseService _database;

    public TaskRepository(DatabaseService database)
    {
        _database = database;
    }

    public async Task<List<TaskItem>> GetAllTasksAsync()
    {
        return await _database.GetAllTasksAsync();
    }

    public async Task<TaskItem?> GetTaskByIdAsync(int id)
    {
        return await _database.GetTaskByIdAsync(id);
    }

    public async Task<int> SaveTaskAsync(TaskItem task)
    {
        return await _database.SaveTaskAsync(task);
    }

    public async Task<int> DeleteTaskAsync(TaskItem task)
    {
        return await _database.DeleteTaskAsync(task);
    }

    public async Task<int> ToggleTaskCompletionAsync(TaskItem task)
    {
        task.IsCompleted = !task.IsCompleted;
        return await _database.SaveTaskAsync(task);
    }

    public async Task<List<TaskItem>> GetTasksForTodayAsync()
    {
        var today = DateTime.Today;
        var allTasks = await _database.GetAllTasksAsync();
        
        return allTasks.Where(t => t.Deadline.Date == today).ToList();
    }

    public async Task<List<TaskItem>> GetOverdueTasksAsync()
    {
        return await _database.GetOverdueTasksAsync();
    }

    public async Task<List<TaskItem>> GetTasksByWeekAsync(DateTime weekStart)
    {
        var weekEnd = weekStart.AddDays(6);
        return await _database.GetTasksByWeekAsync(weekStart, weekEnd);
    }
}
