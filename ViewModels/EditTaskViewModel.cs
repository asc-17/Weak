using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Weak.Models;
using Weak.Services;

namespace Weak.ViewModels;

[QueryProperty(nameof(TaskId), "taskId")]
public partial class EditTaskViewModel : ObservableObject
{
    private readonly TaskRepository _taskRepository;
    private readonly INotificationService _notificationService;
    private TaskItem? _currentTask;

    [ObservableProperty]
    private int taskId;

    [ObservableProperty]
    private string taskTitle = string.Empty;

    [ObservableProperty]
    private string subject = string.Empty;

    [ObservableProperty]
    private string category = string.Empty;

    [ObservableProperty]
    private DateTime taskDate = DateTime.Today;

    [ObservableProperty]
    private double effort = 5;

    [ObservableProperty]
    private double completionPercent = 0;

    [ObservableProperty]
    private string recurrenceType = "none";

    [ObservableProperty]
    private int recurrenceInterval = 1;

    [ObservableProperty]
    private bool isDayOnly = false;

    public bool IsNoneSelected => RecurrenceType == "none";
    public bool IsDailySelected => RecurrenceType == "daily";
    public bool IsWeeklySelected => RecurrenceType == "weekly";
    public bool IsMonthlySelected => RecurrenceType == "monthly";
    public bool IsCustomSelected => RecurrenceType == "custom";

    public EditTaskViewModel(TaskRepository taskRepository, INotificationService notificationService)
    {
        _taskRepository = taskRepository;
        _notificationService = notificationService;
    }

    [RelayCommand]
    private void SetRecurrence(string type)
    {
        RecurrenceType = type;
    }

    partial void OnRecurrenceTypeChanged(string value)
    {
        OnPropertyChanged(nameof(IsNoneSelected));
        OnPropertyChanged(nameof(IsDailySelected));
        OnPropertyChanged(nameof(IsWeeklySelected));
        OnPropertyChanged(nameof(IsMonthlySelected));
        OnPropertyChanged(nameof(IsCustomSelected));
    }

    async partial void OnTaskIdChanged(int value)
    {
        await LoadTaskAsync(value);
    }

    private async Task LoadTaskAsync(int taskId)
    {
        _currentTask = await _taskRepository.GetTaskByIdAsync(taskId);
        
        if (_currentTask != null)
        {
            taskTitle = _currentTask.Title;
            subject = _currentTask.Subject ?? string.Empty;
            category = _currentTask.Category ?? string.Empty;
            taskDate = _currentTask.Deadline;
            effort = _currentTask.Effort;
            completionPercent = _currentTask.CompletionPercent;
            recurrenceType = _currentTask.RecurrenceType;
            recurrenceInterval = _currentTask.RecurrenceInterval;
            isDayOnly = _currentTask.IsDayOnly;

            OnPropertyChanged(nameof(TaskTitle));
            OnPropertyChanged(nameof(Subject));
            OnPropertyChanged(nameof(Category));
            OnPropertyChanged(nameof(TaskDate));
            OnPropertyChanged(nameof(Effort));
            OnPropertyChanged(nameof(CompletionPercent));
            OnPropertyChanged(nameof(RecurrenceType));
            OnPropertyChanged(nameof(RecurrenceInterval));
            OnPropertyChanged(nameof(IsDayOnly));
            OnPropertyChanged(nameof(IsNoneSelected));
            OnPropertyChanged(nameof(IsDailySelected));
            OnPropertyChanged(nameof(IsWeeklySelected));
            OnPropertyChanged(nameof(IsMonthlySelected));
            OnPropertyChanged(nameof(IsCustomSelected));
        }
    }

    [RelayCommand]
    private async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task SaveTask()
    {
        if (_currentTask == null)
            return;

        if (string.IsNullOrWhiteSpace(taskTitle))
        {
            await Application.Current!.MainPage!.DisplayAlert(
                "Validation Error",
                "Please enter a task name.",
                "OK");
            return;
        }

        _currentTask.Title = taskTitle;
        _currentTask.Subject = string.IsNullOrWhiteSpace(subject) ? null : subject;
        _currentTask.Category = string.IsNullOrWhiteSpace(category) ? null : category;
        _currentTask.Deadline = taskDate;
        _currentTask.Effort = (int)Math.Round(effort);
        _currentTask.CompletionPercent = completionPercent;
        _currentTask.RecurrenceType = recurrenceType;
        _currentTask.RecurrenceInterval = recurrenceInterval;
        _currentTask.IsDayOnly = isDayOnly;

        await _taskRepository.SaveTaskAsync(_currentTask);

        if (_notificationService != null && _currentTask.CompletionPercent < 100)
        {
            await _notificationService.ScheduleTaskDeadlineNotificationAsync(_currentTask);
        }

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task DeleteTask()
    {
        if (_currentTask == null)
            return;

        var confirm = await Application.Current!.MainPage!.DisplayAlert(
            "Delete Task",
            "Are you sure you want to delete this task?",
            "Delete",
            "Cancel");

        if (confirm)
        {
            await _taskRepository.DeleteTaskAsync(_currentTask);
            await Shell.Current.GoToAsync("..");
        }
    }
}

