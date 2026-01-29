namespace Weak.Models;

public class CalendarItem
{
    public string Time { get; set; }
    public string Period { get; set; } // AM/PM
    public string Title { get; set; }
    public string Subtitle { get; set; }
    public string PriorityColor { get; set; }
    public bool IsPriority { get; set; }
}
