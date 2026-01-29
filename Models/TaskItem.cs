using CommunityToolkit.Mvvm.ComponentModel;

namespace Weak.Models;

public partial class TaskItem : ObservableObject
{
    [ObservableProperty]
    private string title;

    [ObservableProperty]
    private DateTime deadline;

    [ObservableProperty]
    private bool isCompleted;

    [ObservableProperty]
    private string subjectColor; // Hex string

    // Display properties for task deadline
    public string DeadlineText
    {
        get
        {
            if (isCompleted)
                return "Done";
            
            var today = DateTime.Today;
            if (deadline.Date == today)
                return "Today";
            
            if (deadline.Date == today.AddDays(1))
                return "Tomorrow";
            
            // Check if it's today with time
            if (deadline.Date == today && deadline.TimeOfDay != TimeSpan.Zero)
                return deadline.ToString("ddd, h:mm tt");
            
            return deadline.ToString("ddd, h:mm tt");
        }
    }

    public string DeadlineColor
    {
        get
        {
            if (isCompleted)
                return "#9ca3af"; // Gray
            
            var today = DateTime.Today;
            if (deadline.Date == today)
                return "#ef4444"; // Red for today
            
            return "#64748b"; // Default gray
        }
    }
}

