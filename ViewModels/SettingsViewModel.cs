using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.Json;
using Weak.Services;

namespace Weak.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly CalendarImportService _calendarImport;
    private readonly TaskRepository _taskRepository;
    private readonly DatabaseService _database;
    private readonly SettingsService _settingsService;

    private bool _isInitializing;

    [ObservableProperty]
    private string userName = string.Empty;

    [ObservableProperty]
    private TimeSpan wakeTime = new TimeSpan(7, 0, 0);

    [ObservableProperty]
    private TimeSpan sleepTime = new TimeSpan(22, 0, 0);

    [ObservableProperty]
    private string weekStartDay = "sunday";

    [ObservableProperty]
    private bool notifWeeklyOverview;

    [ObservableProperty]
    private bool notifDeadlineReminder;

    [ObservableProperty]
    private bool notificationsEnabled = true;

    [ObservableProperty]
    private bool calendarSyncEnabled = false;

    [ObservableProperty]
    private string lastSyncTime = "Never";

    public bool IsSundaySelected => WeekStartDay == "sunday";
    public bool IsMondaySelected => WeekStartDay == "monday";

    public SettingsViewModel(
        CalendarImportService calendarImport,
        TaskRepository taskRepository,
        DatabaseService database,
        SettingsService settingsService)
    {
        _calendarImport = calendarImport;
        _taskRepository = taskRepository;
        _database = database;
        _settingsService = settingsService;
    }

    public async Task InitializeAsync()
    {
        _isInitializing = true;
        try
        {
            var settings = await _settingsService.GetSettingsAsync();
            UserName = settings.Name ?? string.Empty;
            WakeTime = settings.WakeTime;
            SleepTime = settings.SleepTime;
            WeekStartDay = settings.WeekStartDay ?? "sunday";
            NotifWeeklyOverview = settings.NotifWeeklyOverview;
            NotifDeadlineReminder = settings.NotifDeadlineReminder;
            NotificationsEnabled = Preferences.Get("notifications_enabled", true);
            CalendarSyncEnabled = Preferences.Get("calendar_sync_enabled", false);
            LastSyncTime = Preferences.Get("last_sync_time", "Never");
        }
        finally
        {
            _isInitializing = false;
        }
    }

    private async Task SaveAsync()
    {
        if (_isInitializing) return;
        var settings = await _settingsService.GetSettingsAsync();
        settings.Name = UserName;
        settings.WakeTime = WakeTime;
        settings.SleepTime = SleepTime;
        settings.WeekStartDay = WeekStartDay;
        settings.NotifWeeklyOverview = NotifWeeklyOverview;
        settings.NotifDeadlineReminder = NotifDeadlineReminder;
        await _settingsService.SaveSettingsAsync(settings);
    }

    [RelayCommand]
    private void SetWeekStart(string day)
    {
        WeekStartDay = day;
    }

    partial void OnUserNameChanged(string value) => _ = SaveAsync();
    partial void OnWakeTimeChanged(TimeSpan value) => _ = SaveAsync();
    partial void OnSleepTimeChanged(TimeSpan value) => _ = SaveAsync();

    partial void OnWeekStartDayChanged(string value)
    {
        OnPropertyChanged(nameof(IsSundaySelected));
        OnPropertyChanged(nameof(IsMondaySelected));
        _ = SaveAsync();
    }

    partial void OnNotifWeeklyOverviewChanged(bool value) => _ = SaveAsync();
    partial void OnNotifDeadlineReminderChanged(bool value) => _ = SaveAsync();

    partial void OnNotificationsEnabledChanged(bool value)
    {
        Preferences.Set("notifications_enabled", value);
        if (!value)
        {
            NotifWeeklyOverview = false;
            NotifDeadlineReminder = false;
        }
    }

    partial void OnCalendarSyncEnabledChanged(bool value)
    {
        Preferences.Set("calendar_sync_enabled", value);
        if (value)
            _ = SyncCalendarAsync();
    }

    [RelayCommand]
    private async Task SyncCalendar()
    {
        await SyncCalendarAsync();
    }

    private async Task SyncCalendarAsync()
    {
        try
        {
            var startDate = DateTime.Today;
            var endDate = startDate.AddMonths(3);

            var events = await _calendarImport.GetDeviceCalendarEventsAsync(startDate, endDate);
            await _calendarImport.ImportCalendarEventsAsync(startDate, endDate, events);

            LastSyncTime = $"Today {DateTime.Now:h:mm tt}";
            Preferences.Set("last_sync_time", LastSyncTime);
        }
        catch (Exception)
        {
            // Handle error silently
        }
    }

    [RelayCommand]
    private async Task ExportData()
    {
        try
        {
            var tasks = await _taskRepository.GetAllTasksAsync();
            var json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions { WriteIndented = true });

            var fileName = $"weak_export_{DateTime.Now:yyyyMMdd_HHmmss}.json";
            var filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

            await File.WriteAllTextAsync(filePath, json);

            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = "Export Weak Data",
                File = new ShareFile(filePath)
            });
        }
        catch (Exception)
        {
            // Handle error
        }
    }

    [RelayCommand]
    private async Task ImportData()
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Select Weak Data File"
            });

            if (result != null)
            {
                var json = await File.ReadAllTextAsync(result.FullPath);
                var tasks = JsonSerializer.Deserialize<List<Models.TaskItem>>(json);

                if (tasks != null)
                {
                    foreach (var task in tasks)
                    {
                        task.Id = 0;
                        await _taskRepository.SaveTaskAsync(task);
                    }
                }
            }
        }
        catch (Exception)
        {
            // Handle error
        }
    }

    }
