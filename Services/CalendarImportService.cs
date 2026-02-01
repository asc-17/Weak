using Weak.Models;

namespace Weak.Services;

public class CalendarImportService
{
    private readonly DatabaseService _database;

    public CalendarImportService(DatabaseService database)
    {
        _database = database;
    }

    public async Task<int> ImportCalendarEventsAsync(DateTime startDate, DateTime endDate, List<CalendarEvent> events)
    {
        var importedCount = 0;

        foreach (var calendarEvent in events)
        {
            var existingTask = await _database.GetTaskByExternalIdAsync(calendarEvent.Id);

            if (existingTask != null)
            {
                var deadlineChanged = existingTask.Deadline != calendarEvent.EndDate;

                if (deadlineChanged)
                {
                    existingTask.Deadline = calendarEvent.EndDate;
                    await _database.SaveTaskAsync(existingTask);
                }
            }
            else
            {
                var newTask = new TaskItem
                {
                    Title = calendarEvent.Title,
                    Deadline = calendarEvent.EndDate,
                    Effort = 1,
                    CompletionPercent = 0,
                    Source = TaskSource.Calendar,
                    ExternalId = calendarEvent.Id,
                    SubjectColor = "#64748b"
                };

                await _database.SaveTaskAsync(newTask);
                importedCount++;
            }
        }

        return importedCount;
    }

    public async Task<List<CalendarEvent>> GetDeviceCalendarEventsAsync(DateTime startDate, DateTime endDate)
    {
#if ANDROID
        return await GetAndroidCalendarEventsAsync(startDate, endDate);
#else
        return new List<CalendarEvent>();
#endif
    }

#if ANDROID
    private async Task<List<CalendarEvent>> GetAndroidCalendarEventsAsync(DateTime startDate, DateTime endDate)
    {
        var events = new List<CalendarEvent>();

        try
        {
            var hasPermission = await Permissions.CheckStatusAsync<Permissions.CalendarRead>();
            
            if (hasPermission != PermissionStatus.Granted)
            {
                hasPermission = await Permissions.RequestAsync<Permissions.CalendarRead>();
            }

            if (hasPermission != PermissionStatus.Granted)
                return events;

            var context = Platform.CurrentActivity?.ApplicationContext;
            if (context == null)
                return events;

            var eventsUri = Android.Provider.CalendarContract.Events.ContentUri;
            var projection = new[]
            {
                Android.Provider.CalendarContract.Events.InterfaceConsts.Id,
                Android.Provider.CalendarContract.Events.InterfaceConsts.Title,
                Android.Provider.CalendarContract.Events.InterfaceConsts.Dtstart,
                Android.Provider.CalendarContract.Events.InterfaceConsts.Dtend
            };

            var selection = $"({Android.Provider.CalendarContract.Events.InterfaceConsts.Dtstart} >= ? AND {Android.Provider.CalendarContract.Events.InterfaceConsts.Dtstart} <= ?)";
            var selectionArgs = new[]
            {
                new DateTimeOffset(startDate).ToUnixTimeMilliseconds().ToString(),
                new DateTimeOffset(endDate).ToUnixTimeMilliseconds().ToString()
            };

            var cursor = context.ContentResolver?.Query(eventsUri, projection, selection, selectionArgs, null);

            if (cursor != null)
            {
                while (cursor.MoveToNext())
                {
                    var id = cursor.GetString(0) ?? string.Empty;
                    var title = cursor.GetString(1) ?? "Untitled Event";
                    var dtStart = cursor.GetLong(2);
                    var dtEnd = cursor.GetLong(3);

                    events.Add(new CalendarEvent
                    {
                        Id = id,
                        Title = title,
                        StartDate = DateTimeOffset.FromUnixTimeMilliseconds(dtStart).DateTime,
                        EndDate = DateTimeOffset.FromUnixTimeMilliseconds(dtEnd).DateTime
                    });
                }
                cursor.Close();
            }
        }
        catch (Exception)
        {
            // Handle silently
        }

        return events;
    }
#endif
}

public class CalendarEvent
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
