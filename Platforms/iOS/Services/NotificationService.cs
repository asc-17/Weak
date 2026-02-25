using Foundation;
using UserNotifications;
using Weak.Models;
using Weak.Services;

namespace Weak.Platforms.iOS.Services;

public class NotificationService : INotificationService
{
    private readonly SettingsService _settingsService;

    public NotificationService(SettingsService settingsService)
    {
        _settingsService = settingsService;
        RequestPermissions();
    }

    private void RequestPermissions()
    {
        UNUserNotificationCenter.Current.RequestAuthorization(
            UNAuthorizationOptions.Alert | UNAuthorizationOptions.Sound | UNAuthorizationOptions.Badge,
            (granted, error) => { });
    }

    public async Task ScheduleTaskDeadlineNotificationAsync(TaskItem task)
    {
        var settings = await _settingsService.GetSettingsAsync();
        if (!settings.NotifDeadlineReminder)
            return;

        var notificationTime = task.Deadline.AddHours(-24);
        if (notificationTime <= DateTime.Now)
            return;

        var content = new UNMutableNotificationContent
        {
            Title = "Task Due Tomorrow",
            Body = task.Title,
            Sound = UNNotificationSound.Default
        };

        var interval = (notificationTime - DateTime.Now).TotalSeconds;
        if (interval <= 0) return;

        var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(interval, false);
        var request = UNNotificationRequest.FromIdentifier(
            $"deadline_{task.Id}", content, trigger);

        await UNUserNotificationCenter.Current.AddNotificationRequestAsync(request);
    }

    public async Task ScheduleWeeklyLoadWarningAsync(DateTime weekStart, double load)
    {
        var settings = await _settingsService.GetSettingsAsync();
        if (!settings.NotifWeeklyOverview)
            return;

        if (load < 3.0)
            return;

        var message = load >= 8.0
            ? "Critical workload ahead. Plan your week strategically."
            : load >= 6.0
                ? "Heavy workload next week. Stay organized."
                : "Moderate workload next week. You've got this.";

        var content = new UNMutableNotificationContent
        {
            Title = "Weekly Load Warning",
            Body = message,
            Sound = UNNotificationSound.Default
        };

        // Fire Sunday at 7 PM
        var dateComponents = new NSDateComponents
        {
            Weekday = 1, // Sunday = 1 in NSCalendar
            Hour = 19,
            Minute = 0
        };

        var trigger = UNCalendarNotificationTrigger.CreateTrigger(dateComponents, true);
        var request = UNNotificationRequest.FromIdentifier(
            "weekly_warning", content, trigger);

        await UNUserNotificationCenter.Current.AddNotificationRequestAsync(request);
    }

    public Task CancelNotificationAsync(int notificationId)
    {
        UNUserNotificationCenter.Current.RemovePendingNotificationRequests(
            new[] { $"deadline_{notificationId}" });
        return Task.CompletedTask;
    }

    public Task CancelAllNotificationsAsync()
    {
        UNUserNotificationCenter.Current.RemoveAllPendingNotificationRequests();
        return Task.CompletedTask;
    }
}
