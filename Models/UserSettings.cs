using SQLite;

namespace Weak.Models;

public class UserSettings
{
    [PrimaryKey]
    public int Id { get; set; } = 1;

    public string Name { get; set; } = string.Empty;

    [Ignore]
    public TimeSpan WakeTime
    {
        get => TimeSpan.TryParse(WakeTimeText, out var ts) ? ts : TimeSpan.FromHours(7);
        set => WakeTimeText = value.ToString(@"hh\:mm");
    }

    [Column("wake_time")]
    public string WakeTimeText { get; set; } = "07:00";

    [Ignore]
    public TimeSpan SleepTime
    {
        get => TimeSpan.TryParse(SleepTimeText, out var ts) ? ts : TimeSpan.FromHours(22);
        set => SleepTimeText = value.ToString(@"hh\:mm");
    }

    [Column("sleep_time")]
    public string SleepTimeText { get; set; } = "22:00";

    public string WeekStartDay { get; set; } = "sunday";

    public bool GoogleConnected { get; set; } = false;

    public bool NotifWeeklyOverview { get; set; } = true;

    public bool NotifDeadlineReminder { get; set; } = true;

    public bool OnboardingComplete { get; set; } = false;
}
