using SQLite;
using Weak.Models;

namespace Weak.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _database;

    public async Task InitializeAsync()
    {
        if (_database != null)
            return;

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "weak.db3");
        _database = new SQLiteAsyncConnection(dbPath);

        await _database.CreateTableAsync<TaskItem>();
        await _database.CreateTableAsync<TaskList>();
        await _database.CreateTableAsync<UserSettings>();
    }

    public async Task<List<TaskItem>> GetAllTasksAsync()
    {
        await InitializeAsync();
        return await _database!.Table<TaskItem>().ToListAsync();
    }

    public async Task<TaskItem?> GetTaskByIdAsync(int id)
    {
        await InitializeAsync();
        return await _database!.Table<TaskItem>()
            .Where(t => t.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<List<TaskItem>> GetTasksByWeekAsync(DateTime weekStart, DateTime weekEnd)
    {
        await InitializeAsync();
        var allTasks = await _database!.Table<TaskItem>().ToListAsync();
        
        return allTasks.Where(t => 
            t.Deadline.Date >= weekStart.Date && 
            t.Deadline.Date <= weekEnd.Date)
            .ToList();
    }

    public async Task<int> SaveTaskAsync(TaskItem task)
    {
        await InitializeAsync();
        
        if (task.Id != 0)
        {
            await _database!.UpdateAsync(task);
            return task.Id;
        }
        else
        {
            return await _database!.InsertAsync(task);
        }
    }

    public async Task<int> DeleteTaskAsync(TaskItem task)
    {
        await InitializeAsync();
        return await _database!.DeleteAsync(task);
    }

    public async Task<List<TaskItem>> GetOverdueTasksAsync()
    {
        await InitializeAsync();
        var today = DateTime.Today;
        var allTasks = await _database!.Table<TaskItem>().ToListAsync();
        
        return allTasks.Where(t => 
            t.Deadline.Date < today && 
            t.CompletionPercent < 100)
            .ToList();
    }

    public async Task<TaskItem?> GetTaskByExternalIdAsync(string externalId)
    {
        await InitializeAsync();
        return await _database!.Table<TaskItem>()
            .Where(t => t.ExternalId == externalId)
            .FirstOrDefaultAsync();
    }

    // TaskList CRUD
    public async Task<List<TaskList>> GetAllTaskListsAsync()
    {
        await InitializeAsync();
        return await _database!.Table<TaskList>().ToListAsync();
    }

    public async Task<TaskList?> GetTaskListByIdAsync(int id)
    {
        await InitializeAsync();
        return await _database!.Table<TaskList>()
            .Where(l => l.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<List<TaskItem>> GetSubtasksForListAsync(int listId)
    {
        await InitializeAsync();
        var all = await _database!.Table<TaskItem>().ToListAsync();
        return all.Where(t => t.ParentListId == listId).ToList();
    }

    public async Task<int> SaveTaskListAsync(TaskList list)
    {
        await InitializeAsync();
        if (list.Id != 0)
        {
            await _database!.UpdateAsync(list);
            return list.Id;
        }
        return await _database!.InsertAsync(list);
    }

    public async Task DeleteTaskListAsync(TaskList list)
    {
        await InitializeAsync();
        var subtasks = await GetSubtasksForListAsync(list.Id);
        foreach (var task in subtasks)
            await _database!.DeleteAsync(task);
        await _database!.DeleteAsync(list);
    }

    // UserSettings CRUD
    public async Task<UserSettings> GetUserSettingsAsync()
    {
        await InitializeAsync();
        var settings = await _database!.Table<UserSettings>()
            .Where(s => s.Id == 1)
            .FirstOrDefaultAsync();

        if (settings == null)
        {
            settings = new UserSettings { Id = 1 };
            await _database!.InsertAsync(settings);
        }

        return settings;
    }

    public async Task SaveUserSettingsAsync(UserSettings settings)
    {
        await InitializeAsync();
        var existing = await _database!.Table<UserSettings>()
            .Where(s => s.Id == 1)
            .FirstOrDefaultAsync();

        if (existing == null)
            await _database!.InsertAsync(settings);
        else
            await _database!.UpdateAsync(settings);
    }
}
