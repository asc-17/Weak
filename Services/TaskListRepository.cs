using Weak.Models;

namespace Weak.Services;

public class TaskListRepository
{
    private readonly DatabaseService _database;

    public TaskListRepository(DatabaseService database)
    {
        _database = database;
    }

    public async Task<List<TaskList>> GetAllTaskListsAsync()
    {
        await _database.InitializeAsync();
        var lists = await _database.GetAllTaskListsAsync();
        foreach (var list in lists)
        {
            await RecalculateListProperties(list);
        }
        return lists;
    }

    public void RecalculateListPropertiesFromSubtasks(TaskList list)
    {
        var subtasks = list.Subtasks;
        list.SubtaskCount = subtasks.Count;
        list.CompletedSubtaskCount = subtasks.Count(t => t.IsCompleted);
        list.AverageEffort = subtasks.Any() ? subtasks.Average(t => t.Effort) : 0;

        var totalEffort = subtasks.Sum(t => t.Effort);
        if (totalEffort > 0)
        {
            var weightedCompletion = subtasks.Sum(t => t.Effort * (t.IsCompleted ? 100.0 : 0.0));
            list.WeightedCompletionPercent = weightedCompletion / totalEffort;
        }
        else
        {
            list.WeightedCompletionPercent = 0;
        }
    }

    public async Task RecalculateListProperties(TaskList list)
    {
        var subtasks = await _database.GetSubtasksForListAsync(list.Id);
        list.Subtasks = subtasks;
        RecalculateListPropertiesFromSubtasks(list);
    }

    public async Task<TaskList?> GetTaskListByIdAsync(int id)
        => await _database.GetTaskListByIdAsync(id);

    public async Task<int> SaveTaskListAsync(TaskList list)
        => await _database.SaveTaskListAsync(list);

    public async Task DeleteTaskListAsync(TaskList list)
        => await _database.DeleteTaskListAsync(list);

    public async Task<List<TaskItem>> GetSubtasksAsync(int listId)
        => await _database.GetSubtasksForListAsync(listId);
}
