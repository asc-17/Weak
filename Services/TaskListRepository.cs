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
            var subtasks = await _database.GetSubtasksForListAsync(list.Id);
            list.Subtasks = subtasks;
            list.SubtaskCount = subtasks.Count;
            list.CompletedSubtaskCount = subtasks.Count(t => t.IsCompleted);
            list.AverageEffort = subtasks.Any() ? subtasks.Average(t => t.Effort) : 0;
        }
        return lists;
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
