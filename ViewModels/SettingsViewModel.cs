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

    [ObservableProperty]
    private bool notificationsEnabled = true;

    [ObservableProperty]
    private bool calendarSyncEnabled = false;

    [ObservableProperty]
    private string lastSyncTime = "Never";

    public SettingsViewModel(
        CalendarImportService calendarImport, 
        TaskRepository taskRepository,
        DatabaseService database)
    {
        _calendarImport = calendarImport;
        _taskRepository = taskRepository;
        _database = database;
        LoadSettings();
    }

    private void LoadSettings()
    {
        NotificationsEnabled = Preferences.Get("notifications_enabled", true);
        CalendarSyncEnabled = Preferences.Get("calendar_sync_enabled", false);
        LastSyncTime = Preferences.Get("last_sync_time", "Never");
    }

    partial void OnNotificationsEnabledChanged(bool value)
    {
        Preferences.Set("notifications_enabled", value);
    }

    partial void OnCalendarSyncEnabledChanged(bool value)
    {
        Preferences.Set("calendar_sync_enabled", value);
        
        if (value)
        {
            _ = SyncCalendarAsync();
        }
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

    [RelayCommand]
    private async Task ResetSettings()
    {
        var confirm = await Application.Current!.MainPage!.DisplayAlert(
            "Reset All Settings",
            "This will delete all tasks and reset all preferences. Continue?",
            "Reset",
            "Cancel");

        if (confirm)
        {
            Preferences.Clear();
            
            var allTasks = await _taskRepository.GetAllTasksAsync();
            foreach (var task in allTasks)
            {
                await _taskRepository.DeleteTaskAsync(task);
            }

            LoadSettings();
        }
    }
}
