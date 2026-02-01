using Android.App;
using Android.Content;
using AndroidX.Core.App;
using Weak.Models;
using Weak.Services;

namespace Weak.Platforms.Android.Services;

public class NotificationService : INotificationService
{
    private const string ChannelId = "weak_notifications";
    private const string ChannelName = "Task Notifications";
    private const int DeadlineNotificationId = 1000;
    private const int WeeklyWarningNotificationId = 2000;

    private readonly NotificationManagerCompat _notificationManager;
    private readonly Context _context;

    public NotificationService()
    {
        _context = Platform.CurrentActivity?.ApplicationContext 
            ?? throw new InvalidOperationException("Android context not available");
        
        _notificationManager = NotificationManagerCompat.From(_context);
        CreateNotificationChannel();
    }

    private void CreateNotificationChannel()
    {
        if (OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            var channel = new NotificationChannel(ChannelId, ChannelName, NotificationImportance.Default)
            {
                Description = "Notifications for task deadlines and weekly load warnings"
            };
            
            var manager = (NotificationManager?)_context.GetSystemService(Context.NotificationService);
            manager?.CreateNotificationChannel(channel);
        }
    }

    public Task ScheduleTaskDeadlineNotificationAsync(TaskItem task)
    {
        var notificationTime = task.Deadline.AddHours(-24);
        
        if (notificationTime <= DateTime.Now)
            return Task.CompletedTask;

        var builder = new NotificationCompat.Builder(_context, ChannelId)
            .SetSmallIcon(global::Android.Resource.Drawable.IcMenuInfoDetails)
            .SetContentTitle("Task Due Tomorrow")
            .SetContentText(task.Title)
            .SetPriority(NotificationCompat.PriorityDefault)
            .SetAutoCancel(true);

        var alarmManager = (AlarmManager?)_context.GetSystemService(Context.AlarmService);
        var intent = new Intent(_context, typeof(NotificationReceiver));
        intent.PutExtra("notification_id", DeadlineNotificationId + task.Id);
        intent.PutExtra("title", "Task Due Tomorrow");
        intent.PutExtra("message", task.Title);

        var pendingIntent = PendingIntent.GetBroadcast(
            _context, 
            DeadlineNotificationId + task.Id, 
            intent, 
            PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

        var triggerTime = (long)(notificationTime - DateTime.UnixEpoch).TotalMilliseconds;
        
        if (OperatingSystem.IsAndroidVersionAtLeast(23))
        {
            alarmManager?.SetExactAndAllowWhileIdle(AlarmType.RtcWakeup, triggerTime, pendingIntent);
        }
        else
        {
            alarmManager?.SetExact(AlarmType.RtcWakeup, triggerTime, pendingIntent);
        }

        return Task.CompletedTask;
    }

    public Task ScheduleWeeklyLoadWarningAsync(DateTime weekStart, double load)
    {
        if (load < 40)
            return Task.CompletedTask;

        var saturday = weekStart.AddDays(-1);
        var notificationTime = new DateTime(saturday.Year, saturday.Month, saturday.Day, 9, 0, 0);

        if (notificationTime <= DateTime.Now)
            return Task.CompletedTask;

        var message = load >= 60 
            ? "Heavy workload ahead. Plan your week strategically." 
            : "Moderate to high workload next week. Stay organized.";

        var alarmManager = (AlarmManager?)_context.GetSystemService(Context.AlarmService);
        var intent = new Intent(_context, typeof(NotificationReceiver));
        intent.PutExtra("notification_id", WeeklyWarningNotificationId);
        intent.PutExtra("title", "Weekly Load Warning");
        intent.PutExtra("message", message);

        var pendingIntent = PendingIntent.GetBroadcast(
            _context, 
            WeeklyWarningNotificationId, 
            intent, 
            PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

        var triggerTime = (long)(notificationTime - DateTime.UnixEpoch).TotalMilliseconds;
        
        if (OperatingSystem.IsAndroidVersionAtLeast(23))
        {
            alarmManager?.SetExactAndAllowWhileIdle(AlarmType.RtcWakeup, triggerTime, pendingIntent);
        }
        else
        {
            alarmManager?.SetExact(AlarmType.RtcWakeup, triggerTime, pendingIntent);
        }

        return Task.CompletedTask;
    }

    public Task CancelNotificationAsync(int notificationId)
    {
        _notificationManager.Cancel(notificationId);
        return Task.CompletedTask;
    }

    public Task CancelAllNotificationsAsync()
    {
        _notificationManager.CancelAll();
        return Task.CompletedTask;
    }
}

[BroadcastReceiver(Enabled = true)]
public class NotificationReceiver : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        if (context == null || intent == null)
            return;

        var notificationId = intent.GetIntExtra("notification_id", 0);
        var title = intent.GetStringExtra("title") ?? "Weak";
        var message = intent.GetStringExtra("message") ?? "";

        var builder = new NotificationCompat.Builder(context, "weak_notifications")
            .SetSmallIcon(global::Android.Resource.Drawable.IcMenuInfoDetails)
            .SetContentTitle(title)
            .SetContentText(message)
            .SetPriority(NotificationCompat.PriorityDefault)
            .SetAutoCancel(true);

        var notificationManager = NotificationManagerCompat.From(context);
        notificationManager.Notify(notificationId, builder.Build());
    }
}
