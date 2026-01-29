using System.Collections.ObjectModel;

namespace Weak.Models;

public class CalendarGroup : ObservableCollection<CalendarItem>
{
    public string DayName { get; set; } // Mon, Tue
    public string DayNumber { get; set; } // 23, 24
    public string Title { get; set; } // Today, Tomorrow
    public bool IsToday { get; set; } // For styling the current day
    public int TaskCount => this.Count; // Fixed property name to avoid conflict

    public CalendarGroup(string dayName, string dayNumber, string title, bool isToday, IEnumerable<CalendarItem> items) : base(items)
    {
        DayName = dayName;
        DayNumber = dayNumber;
        Title = title;
        IsToday = isToday;
    }
}
