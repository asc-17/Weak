using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Weak.Models;
using Weak.Services;

namespace Weak.ViewModels;

public partial class TasksViewModel : ObservableObject
{
    private readonly TaskRepository _taskRepository;
    private readonly INotificationService _notificationService;

    public ObservableCollection<TaskItem> Tasks { get; } = new();
    public ObservableCollection<Subject> Subjects { get; } = new();

    [ObservableProperty]
    private string currentDate = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TaskCountDisplay))]
    private int taskCount;

    public string TaskCountDisplay => $"Tasks ({TaskCount})";

    public TasksViewModel(TaskRepository taskRepository, INotificationService notificationService)
    {
        _taskRepository = taskRepository;
        _notificationService = notificationService;
        CurrentDate = DateTime.Now.ToString("dddd, MMM dd");
    }

    public async Task InitializeAsync()
    {
        await LoadTasksAsync();
    }

    public async Task LoadTasksAsync()
    {
        Tasks.Clear();
        
        var allTasks = await _taskRepository.GetAllTasksAsync();
        
        var sortedTasks = allTasks
            .OrderBy(t => t.IsCompleted)
            .ThenBy(t => t.Deadline)
            .ToList();

        foreach (var task in sortedTasks)
        {
            Tasks.Add(task);
        }

        TaskCount = Tasks.Count;
    }

    [RelayCommand]
    private async Task ToggleTask(TaskItem task)
    {
        if (task != null)
        {
            task.IsCompleted = !task.IsCompleted;
            await _taskRepository.SaveTaskAsync(task);
        }
    }

    [RelayCommand]
    private async Task AddTask()
    {
        await Shell.Current.GoToAsync("createtask");
    }

    [RelayCommand]
    private async Task EditTask(TaskItem task)
    {
        if (task != null)
        {
            await Shell.Current.GoToAsync($"edittask?taskId={task.Id}");
        }
    }

    [RelayCommand]
    private async Task DeleteTask(TaskItem task)
    {
        if (task != null)
        {
            await _taskRepository.DeleteTaskAsync(task);
            await LoadTasksAsync();
        }
    }
}

