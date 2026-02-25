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
    private readonly WeekComputationService _weekComputation;

    public ObservableCollection<TaskGroup> TaskGroups { get; } = new();
    public ObservableCollection<Subject> Subjects { get; } = new();

    [ObservableProperty]
    private string currentDate = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TaskCountDisplay))]
    private int taskCount;

    public string TaskCountDisplay => $"Tasks ({TaskCount})";

    public TasksViewModel(TaskRepository taskRepository, INotificationService notificationService, WeekComputationService weekComputation)
    {
        _taskRepository = taskRepository;
        _notificationService = notificationService;
        _weekComputation = weekComputation;
        CurrentDate = DateTime.Now.ToString("dddd, MMM dd");
    }

    public async Task InitializeAsync()
    {
        await LoadTasksAsync();
    }

    public async Task LoadTasksAsync()
    {
        TaskGroups.Clear();

        var allTasks = await _taskRepository.GetAllTasksAsync();
        var today = DateTime.Today;
        var thisWeekStart = await _weekComputation.GetWeekStartAsync(today);

        var overdue = allTasks
            .Where(t => t.Deadline.Date < thisWeekStart)
            .OrderBy(t => t.Deadline).ToList();
        var thisWeek = allTasks
            .Where(t => t.Deadline.Date >= thisWeekStart && t.Deadline.Date < thisWeekStart.AddDays(7))
            .OrderBy(t => t.IsCompleted).ThenBy(t => t.Deadline).ToList();
        var nextWeek = allTasks
            .Where(t => t.Deadline.Date >= thisWeekStart.AddDays(7) && t.Deadline.Date < thisWeekStart.AddDays(14))
            .OrderBy(t => t.IsCompleted).ThenBy(t => t.Deadline).ToList();
        var weekAfter = allTasks
            .Where(t => t.Deadline.Date >= thisWeekStart.AddDays(14) && t.Deadline.Date < thisWeekStart.AddDays(21))
            .OrderBy(t => t.IsCompleted).ThenBy(t => t.Deadline).ToList();
        var beyond = allTasks
            .Where(t => t.Deadline.Date >= thisWeekStart.AddDays(21))
            .OrderBy(t => t.Deadline).ToList();

        if (overdue.Any())
            TaskGroups.Add(new TaskGroup("Overdue", thisWeekStart.AddDays(-7), overdue));

        if (thisWeek.Any())
            TaskGroups.Add(new TaskGroup(
                $"This Week \u00b7 {thisWeekStart:MMM d} \u2013 {thisWeekStart.AddDays(6):MMM d}",
                thisWeekStart, thisWeek));

        if (nextWeek.Any())
        {
            var nws = thisWeekStart.AddDays(7);
            TaskGroups.Add(new TaskGroup(
                $"Next Week \u00b7 {nws:MMM d} \u2013 {nws.AddDays(6):MMM d}",
                nws, nextWeek));
        }

        if (weekAfter.Any())
        {
            var was = thisWeekStart.AddDays(14);
            TaskGroups.Add(new TaskGroup(
                $"Week After \u00b7 {was:MMM d} \u2013 {was.AddDays(6):MMM d}",
                was, weekAfter));
        }

        var beyondByWeek = new Dictionary<DateTime, List<TaskItem>>();
        foreach (var task in beyond)
        {
            var ws = await _weekComputation.GetWeekStartAsync(task.Deadline);
            if (!beyondByWeek.ContainsKey(ws))
                beyondByWeek[ws] = new List<TaskItem>();
            beyondByWeek[ws].Add(task);
        }
        foreach (var (ws, items) in beyondByWeek.OrderBy(kv => kv.Key))
            TaskGroups.Add(new TaskGroup(
                $"{ws:MMM d} \u2013 {ws.AddDays(6):MMM d}",
                ws, items));

        TaskCount = allTasks.Count;
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
    private void ToggleExpand(TaskItem task)
    {
        if (task != null)
            task.IsExpanded = !task.IsExpanded;
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

