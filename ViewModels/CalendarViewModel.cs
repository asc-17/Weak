using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Weak.Models;
using Weak.Services;

namespace Weak.ViewModels;

public partial class CalendarViewModel : ObservableObject
{
    private readonly TaskRepository _taskRepository;
    private readonly SettingsService _settingsService;
    private readonly WeekComputationService _weekComputationService;

    public ObservableCollection<CalendarGroup> Days { get; } = new();
    public ObservableCollection<CalendarDay> MonthDays { get; } = new();
    public ObservableCollection<TaskItem> SelectedDayTasks { get; } = new();
    public ObservableCollection<object> DayTimelineItems { get; } = new();
    public ObservableCollection<CalendarDay> WeekDays { get; } = new();

    [ObservableProperty]
    private string currentMonth = string.Empty;

    [ObservableProperty]
    private string currentWeek = string.Empty;

    [ObservableProperty]
    private DateTime selectedDate = DateTime.Today;

    [ObservableProperty]
    private DateTime displayMonth = DateTime.Today;

    [ObservableProperty]
    private string viewMode = "Day"; // Timeline, Day, Month

    [ObservableProperty]
    private string selectedDayTitle = string.Empty;

    [ObservableProperty]
    private TimeSpan wakeTime = new(7, 0, 0);

    [ObservableProperty]
    private TimeSpan sleepTime = new(23, 0, 0);

    public CalendarViewModel(TaskRepository taskRepository, SettingsService settingsService, WeekComputationService weekComputationService)
    {
        _taskRepository = taskRepository;
        _settingsService = settingsService;
        _weekComputationService = weekComputationService;
    }

    public async Task InitializeAsync()
    {
        var settings = await _settingsService.GetSettingsAsync();
        WakeTime = settings.WakeTime;
        SleepTime = settings.SleepTime;

        await LoadCalendarDataAsync();
    }

    private async Task LoadCalendarDataAsync()
    {
        await LoadTimelineViewAsync();
        await LoadMonthViewAsync();
        UpdateHeaderInfo();
    }

    [RelayCommand]
    private async Task SwitchView(string mode)
    {
        ViewMode = mode;

        if (mode == "Timeline")
            await LoadTimelineViewAsync();
        else if (mode == "Day")
            await LoadDayViewAsync();
        else if (mode == "Month")
            await LoadMonthViewAsync();
    }

    [RelayCommand]
    private async Task PreviousMonth()
    {
        DisplayMonth = DisplayMonth.AddMonths(-1);
        await LoadMonthViewAsync();
        UpdateHeaderInfo();
    }

    [RelayCommand]
    private async Task NextMonth()
    {
        DisplayMonth = DisplayMonth.AddMonths(1);
        await LoadMonthViewAsync();
        UpdateHeaderInfo();
    }

    [RelayCommand]
    private async Task SelectDay(CalendarDay day)
    {
        // Deselect previous
        foreach (var d in MonthDays)
            d.IsSelected = false;

        day.IsSelected = true;
        SelectedDate = day.Date;
        
        // Update selected day title
        SelectedDayTitle = GetDayTitle(day.Date);
        
        // Load tasks for selected day
        await LoadSelectedDayTasksAsync();
    }

    private async Task LoadSelectedDayTasksAsync()
    {
        SelectedDayTasks.Clear();
        DayTimelineItems.Clear();

        var allTasks = await _taskRepository.GetAllTasksAsync();
        var tasksForDay = allTasks
            .Where(t => t.Deadline.Date == SelectedDate.Date)
            .OrderBy(t => t.Deadline.TimeOfDay)
            .ToList();

        foreach (var task in tasksForDay)
        {
            SelectedDayTasks.Add(task);
        }

        // Build mixed timeline with TimeMarker objects for Day view
        var timelineItems = new List<(TimeSpan SortTime, object Item)>();

        // Add wake/sleep markers
        timelineItems.Add((WakeTime, new TimeMarker { Label = "Wake Up", Time = WakeTime, Color = "#94a3b8" }));
        timelineItems.Add((SleepTime, new TimeMarker { Label = "Sleep", Time = SleepTime, Color = "#334155" }));

        // Add tasks (day view includes IsDayOnly tasks)
        foreach (var task in tasksForDay)
        {
            timelineItems.Add((task.Deadline.TimeOfDay, task));
        }

        foreach (var item in timelineItems.OrderBy(x => x.SortTime))
        {
            DayTimelineItems.Add(item.Item);
        }
    }

    private void UpdateHeaderInfo()
    {
        CurrentMonth = DisplayMonth.ToString("MMMM yyyy");
        CurrentWeek = $"Week {GetWeekNumber(DisplayMonth)}";
    }

    private async Task LoadMonthViewAsync()
    {
        MonthDays.Clear();

        var firstDayOfMonth = new DateTime(DisplayMonth.Year, DisplayMonth.Month, 1);
        var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

        // Start from Sunday of the week containing the first day
        var startDate = firstDayOfMonth.AddDays(-(int)firstDayOfMonth.DayOfWeek);

        // End on Saturday of the week containing the last day
        var endDate = lastDayOfMonth.AddDays(6 - (int)lastDayOfMonth.DayOfWeek);

        // Get all tasks for this period, excluding day-only tasks from month view
        var allTasks = await _taskRepository.GetAllTasksAsync();
        var tasksInPeriod = allTasks
            .Where(t => !t.IsDayOnly && t.Deadline.Date >= startDate && t.Deadline.Date <= endDate)
            .GroupBy(t => t.Deadline.Date)
            .ToDictionary(g => g.Key, g => g.ToList());

        // Generate calendar days
        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            var calendarDay = new CalendarDay(date, date.Month == DisplayMonth.Month);

            if (tasksInPeriod.TryGetValue(date.Date, out var tasksOnDay))
            {
                calendarDay.TaskCount = tasksOnDay.Count;
                calendarDay.HasHighPriorityTask = tasksOnDay.Any(t => t.Deadline.Date <= DateTime.Today.AddDays(1));

                // Calculate LoadScore for the day using the same formula
                var loadScore = _weekComputationService.CalculateLoadScore(tasksOnDay);
                calendarDay.LoadScore = loadScore;
                calendarDay.HasEffort = tasksOnDay.Any(t => t.Effort > 0);

                // Set intensity dot colour based on PRD thresholds
                calendarDay.DayIntensityColor = GetDayIntensityColor(loadScore);

                // Build up to 3 indicator dots for different urgency levels
                calendarDay.TaskIndicatorColors.Clear();
                var urgencyLevels = tasksOnDay
                    .Select(t => GetDayIntensityColor(_weekComputationService.CalculateLoadScore(new List<TaskItem> { t })))
                    .Distinct()
                    .Take(3);
                foreach (var color in urgencyLevels)
                    calendarDay.TaskIndicatorColors.Add(color);
            }

            // Auto-select today
            if (date.Date == SelectedDate.Date)
            {
                calendarDay.IsSelected = true;
            }

            MonthDays.Add(calendarDay);
        }

        // Load tasks for selected day
        await LoadSelectedDayTasksAsync();
    }

    private string GetDayIntensityColor(double loadScore)
    {
        if (loadScore <= 3.0)
            return "#3b82f6"; // Blue
        else if (loadScore <= 6.0)
            return "#eab308"; // Yellow/Amber
        else if (loadScore <= 8.5)
            return "#f97316"; // Orange
        else
            return "#ef4444"; // Red
    }

    private async Task LoadDayViewAsync()
    {
        WeekDays.Clear();

        // Get the week containing the selected date
        var weekStart = SelectedDate.AddDays(-(int)SelectedDate.DayOfWeek);

        var allTasks = await _taskRepository.GetAllTasksAsync();
        var tasksInWeek = allTasks
            .Where(t => t.Deadline.Date >= weekStart && t.Deadline.Date < weekStart.AddDays(7))
            .GroupBy(t => t.Deadline.Date)
            .ToDictionary(g => g.Key, g => g.ToList());

        // Generate week days
        for (int i = 0; i < 7; i++)
        {
            var date = weekStart.AddDays(i);
            var calendarDay = new CalendarDay(date, true)
            {
                IsSelected = date.Date == SelectedDate.Date
            };

            if (tasksInWeek.TryGetValue(date.Date, out var tasksOnDay))
            {
                calendarDay.TaskCount = tasksOnDay.Count;
            }

            WeekDays.Add(calendarDay);
        }

        // Load tasks for selected day (includes IsDayOnly and markers)
        SelectedDayTitle = GetDayTitle(SelectedDate);
        await LoadSelectedDayTasksAsync();
    }

    [RelayCommand]
    private async Task SelectDayInWeek(CalendarDay day)
    {
        // Deselect previous
        foreach (var d in WeekDays)
            d.IsSelected = false;

        day.IsSelected = true;
        SelectedDate = day.Date;
        SelectedDayTitle = GetDayTitle(day.Date);
        
        // Load tasks for selected day
        await LoadSelectedDayTasksAsync();
    }

    [RelayCommand]
    private async Task ToggleTaskCompletion(TaskItem task)
    {
        task.CompletionPercent = task.CompletionPercent >= 100 ? 0 : 100;
        await _taskRepository.SaveTaskAsync(task);

        // Refresh the current view
        if (ViewMode == "Day")
            await LoadDayViewAsync();
        else if (ViewMode == "Timeline")
            await LoadTimelineViewAsync();
        else if (ViewMode == "Month")
            await LoadMonthViewAsync();
    }

    private async Task LoadWeekViewAsync()
    {
        Days.Clear();

        var weekStart = WeekComputationService.GetWeekStart(SelectedDate);
        var weekEnd = weekStart.AddDays(6);

        var allTasks = await _taskRepository.GetAllTasksAsync();

        var tasksByDate = allTasks
            .Where(t => t.Deadline.Date >= weekStart && t.Deadline.Date <= weekEnd)
            .GroupBy(t => t.Deadline.Date)
            .OrderBy(g => g.Key)
            .ToList();

        foreach (var group in tasksByDate)
        {
            var date = group.Key;
            var dayName = date.ToString("ddd");
            var dayNumber = date.Day.ToString();
            var title = GetDayTitle(date);
            var isToday = date.Date == DateTime.Today;

            var items = group
                .OrderBy(t => t.Deadline.TimeOfDay)
                .Select(t => new CalendarItem
                {
                    Time = t.Deadline.ToString("hh:mm"),
                    Period = t.Deadline.ToString("tt"),
                    Title = t.Title,
                    Subtitle = GetSubtitle(t),
                    PriorityColor = t.SubjectColor,
                    IsPriority = t.Deadline.Date <= DateTime.Today.AddDays(1)
                })
                .ToList();

            Days.Add(new CalendarGroup(dayName, dayNumber, title, isToday, items));
        }
    }

    private async Task LoadTimelineViewAsync()
    {
        Days.Clear();

        var allTasks = await _taskRepository.GetAllTasksAsync();

        // Exclude day-only tasks from timeline view
        var timelineTasks = allTasks.Where(t => !t.IsDayOnly).ToList();

        // Prepend overdue group: tasks before today that are not 100% complete
        var overdueTasks = timelineTasks
            .Where(t => t.Deadline.Date < DateTime.Today && t.CompletionPercent < 100)
            .OrderBy(t => t.Deadline)
            .ToList();

        if (overdueTasks.Any())
        {
            var overdueItems = overdueTasks
                .Select(t => new CalendarItem
                {
                    Time = t.Deadline.ToString("hh:mm"),
                    Period = t.Deadline.ToString("tt"),
                    Title = t.Title,
                    Subtitle = GetSubtitle(t),
                    PriorityColor = t.SubjectColor,
                    IsPriority = true,
                    IsPending = true,
                    Effort = t.Effort,
                    CompletionPercent = t.CompletionPercent,
                    EffortBarColor = GetEffortBarColor(t.Effort)
                })
                .ToList();

            Days.Add(new CalendarGroup("!", "", "Overdue", false, overdueItems));
        }

        // Show tasks from today to 30 days in future
        var startDate = DateTime.Today;
        var endDate = DateTime.Today.AddDays(30);

        var tasksByDate = timelineTasks
            .Where(t => t.Deadline.Date >= startDate && t.Deadline.Date <= endDate)
            .GroupBy(t => t.Deadline.Date)
            .OrderBy(g => g.Key)
            .ToList();

        foreach (var group in tasksByDate)
        {
            var date = group.Key;
            var dayName = date.ToString("ddd");
            var dayNumber = date.Day.ToString();
            var title = GetDayTitle(date);
            var isToday = date.Date == DateTime.Today;

            var items = group
                .OrderBy(t => t.Deadline.TimeOfDay)
                .Select(t => new CalendarItem
                {
                    Time = t.Deadline.ToString("hh:mm"),
                    Period = t.Deadline.ToString("tt"),
                    Title = t.Title,
                    Subtitle = GetSubtitle(t),
                    PriorityColor = t.SubjectColor,
                    IsPriority = t.Deadline.Date <= DateTime.Today.AddDays(1),
                    IsPending = t.Deadline.Date < DateTime.Today && t.CompletionPercent < 100,
                    Effort = t.Effort,
                    CompletionPercent = t.CompletionPercent,
                    EffortBarColor = GetEffortBarColor(t.Effort)
                })
                .ToList();

            Days.Add(new CalendarGroup(dayName, dayNumber, title, isToday, items));
        }

        if (!Days.Any())
        {
            var today = DateTime.Today;
            Days.Add(new CalendarGroup(
                today.ToString("ddd"),
                today.Day.ToString(),
                "Today",
                true,
                new List<CalendarItem>()
            ));
        }
    }

    private string GetEffortBarColor(double effort)
    {
        if (effort <= 3)
            return "#81C784";
        else if (effort <= 6)
            return "#FFB74D";
        else if (effort <= 8)
            return "#FF8A65";
        else
            return "#E57373";
    }

    private string GetDayTitle(DateTime date)
    {
        var daysDiff = (date.Date - DateTime.Today).Days;

        return daysDiff switch
        {
            0 => "Today",
            1 => "Tomorrow",
            -1 => "Yesterday",
            _ => date.ToString("dddd, MMM dd")
        };
    }

    private string GetSubtitle(TaskItem task)
    {
        var parts = new List<string>();

        if (!string.IsNullOrEmpty(task.Subject))
            parts.Add(task.Subject);

        if (!string.IsNullOrEmpty(task.Category))
            parts.Add(task.Category);

        return parts.Any() ? string.Join(" • ", parts) : "No additional details";
    }

    private int GetWeekNumber(DateTime date)
    {
        var culture = System.Globalization.CultureInfo.CurrentCulture;
        var calendar = culture.Calendar;
        var weekRule = culture.DateTimeFormat.CalendarWeekRule;
        var firstDayOfWeek = culture.DateTimeFormat.FirstDayOfWeek;

        return calendar.GetWeekOfYear(date, weekRule, firstDayOfWeek);
    }

    [RelayCommand]
    private async Task NavigateToTask(CalendarItem item)
    {
        var tasks = await _taskRepository.GetAllTasksAsync();
        var task = tasks.FirstOrDefault(t => t.Title == item.Title);

        if (task != null)
        {
            await Shell.Current.GoToAsync($"edittask?taskId={task.Id}");
        }
    }

    [RelayCommand]
    private async Task NavigateToTaskItem(TaskItem task)
    {
        if (task != null)
        {
            await Shell.Current.GoToAsync($"edittask?taskId={task.Id}");
        }
    }
}
