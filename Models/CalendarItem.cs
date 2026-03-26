namespace Weak.Models;

public class CalendarItem
{
    public string Time { get; set; }
    public string Period { get; set; } // AM/PM
    public string Title { get; set; }
    public bool IsPriority { get; set; }
    public bool IsPending { get; set; }
    public int Effort { get; set; }
    public double CompletionPercent { get; set; }
    public string EffortBarColor { get; set; } = "#4CAF50";

    // List-specific properties
    public bool IsListItem { get; set; }
    public string SubtaskProgress { get; set; } = string.Empty;
    public double WeightedCompletionPercent { get; set; }
    public double AverageEffort { get; set; }
}
