using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Weak.Models;
using Weak.Services;

namespace Weak.ViewModels;

public partial class UnifiedCreationViewModel : ObservableObject
{
    private readonly TaskRepository _taskRepository;
    private readonly TaskListRepository _taskListRepository;
    private readonly INotificationService _notificationService;
    private readonly RecurrenceService _recurrenceService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsListMode))]
    [NotifyPropertyChangedFor(nameof(NameLabel))]
    [NotifyPropertyChangedFor(nameof(NamePlaceholder))]
    [NotifyPropertyChangedFor(nameof(CreateButtonText))]
    private bool isTaskMode = true;

    public bool IsListMode => !IsTaskMode;
    public string NameLabel => IsTaskMode ? "TASK NAME" : "LIST NAME";
    public string NamePlaceholder => IsTaskMode ? "e.g., Read Chapter 5" : "e.g., Final Exams";
    public string CreateButtonText => IsTaskMode ? "Create Task" : "Create List";

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string icon = "default_task.svg";

    [ObservableProperty]
    private DateTime dueDate = DateTime.Today.AddDays(1);

    [ObservableProperty]
    private TimeSpan timeOfDay = GetNextRoundTime();

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

    public UnifiedCreationViewModel(
        TaskRepository taskRepository,
        TaskListRepository taskListRepository,
        INotificationService notificationService,
        RecurrenceService recurrenceService)
    {
        _taskRepository = taskRepository;
        _taskListRepository = taskListRepository;
        _notificationService = notificationService;
        _recurrenceService = recurrenceService;
    }

    [RelayCommand]
    private void SwitchToTask() => IsTaskMode = true;

    [RelayCommand]
    private void SwitchToList() => IsTaskMode = false;

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
    private async Task Create()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            await Application.Current!.MainPage!.DisplayAlert(
                "Validation Error",
                IsTaskMode ? "Please enter a task name." : "Please enter a list name.",
                "OK");
            return;
        }

        if (IsTaskMode)
        {
            await CreateTaskAsync();
        }
        else
        {
            await CreateListAsync();
        }

        await Shell.Current.GoToAsync("..");
    }

    private async Task CreateTaskAsync()
    {
        var newTask = new TaskItem
        {
            Title = Name,
            Icon = Icon,
            Deadline = DueDate.Date + TimeOfDay,
            Effort = (int)Math.Round(Effort),
            CompletionPercent = 0,
            Source = TaskSource.Manual,
            RecurrenceType = RecurrenceType,
            RecurrenceInterval = RecurrenceInterval,
            IsDayOnly = IsDayOnly,
            ParentListId = null
        };

        await _taskRepository.SaveTaskAsync(newTask);

        if (newTask.IsRecurring)
        {
            var count = RecurrenceService.GetInstanceCount(newTask);
            var instances = await _recurrenceService.GenerateInstancesAsync(newTask, count);
            foreach (var instance in instances)
                await _taskRepository.SaveTaskAsync(instance);
        }

        if (_notificationService != null)
        {
            await _notificationService.ScheduleTaskDeadlineNotificationAsync(newTask);
        }
    }

    private async Task CreateListAsync()
    {
        var list = new TaskList
        {
            Name = Name,
            DueDate = DueDate.Date + TimeOfDay,
            CreatedAt = DateTime.UtcNow
        };

        await _taskListRepository.SaveTaskListAsync(list);
    }

    private static TimeSpan GetNextRoundTime()
    {
        var now = DateTime.Now;
        var minutes = now.Minute;
        var roundedMinutes = minutes < 30 ? 30 : 0;
        var hour = minutes < 30 ? now.Hour : now.Hour + 1;
        if (hour >= 24) hour = 23;
        return new TimeSpan(hour, roundedMinutes, 0);
    }
}
