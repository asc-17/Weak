using Weak.Models;

namespace Weak.Services;

public interface INotificationService
{
    Task ScheduleTaskDeadlineNotificationAsync(TaskItem task);
    Task ScheduleWeeklyLoadWarningAsync(DateTime weekStart, double load);
    Task CancelNotificationAsync(int notificationId);
    Task CancelAllNotificationsAsync();
}
