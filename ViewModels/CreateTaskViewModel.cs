using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Weak.Models;
using Weak.Services;

namespace Weak.ViewModels;

public partial class CreateTaskViewModel : ObservableObject
{
    private readonly TaskRepository _taskRepository;
    private readonly INotificationService _notificationService;

    [ObservableProperty]
    private string taskTitle = string.Empty;

    [ObservableProperty]
    private string subject = string.Empty;

    [ObservableProperty]
    private string category = string.Empty;

    [ObservableProperty]
    private DateTime taskDate = DateTime.Today.AddDays(1);

    [ObservableProperty]
    private double effort = 5;

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

    public CreateTaskViewModel(TaskRepository taskRepository, INotificationService notificationService)
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

    [RelayCommand]
    private async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task CreateTask()
    {
        if (string.IsNullOrWhiteSpace(taskTitle))
        {
            await Application.Current!.MainPage!.DisplayAlert(
                "Validation Error",
                "Please enter a task name.",
                "OK");
            return;
        }

        var newTask = new TaskItem
        {
            Title = taskTitle,
            Subject = string.IsNullOrWhiteSpace(subject) ? null : subject,
            Category = string.IsNullOrWhiteSpace(category) ? null : category,
            Deadline = taskDate,
            Effort = (int)Math.Round(effort),
            CompletionPercent = 0,
            Source = TaskSource.Manual,
            SubjectColor = GetRandomColor(),
            RecurrenceType = recurrenceType,
            RecurrenceInterval = recurrenceInterval,
            IsDayOnly = isDayOnly
        };

        await _taskRepository.SaveTaskAsync(newTask);

        if (_notificationService != null)
        {
            await _notificationService.ScheduleTaskDeadlineNotificationAsync(newTask);
        }

        await Shell.Current.GoToAsync("..");
    }

    private string GetRandomColor()
    {
        var colors = new[]
        {
            "#ef4444", // Red
            "#3b82f6", // Blue
            "#eab308", // Yellow
            "#8b5cf6", // Purple
            "#10b981", // Green
            "#f97316", // Orange
            "#06b6d4", // Cyan
            "#ec4899"  // Pink
        };

        return colors[Random.Shared.Next(colors.Length)];
    }
}

