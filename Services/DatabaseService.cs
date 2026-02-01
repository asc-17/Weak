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
}
