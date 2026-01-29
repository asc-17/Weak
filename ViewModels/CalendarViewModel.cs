using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Weak.Models;

namespace Weak.ViewModels;

public partial class CalendarViewModel : ObservableObject
{
    public ObservableCollection<CalendarGroup> Days { get; } = new();

    public CalendarViewModel()
    {
        LoadMockData();
    }

    private void LoadMockData()
    {
        Days.Clear();

        // Today
        Days.Add(new CalendarGroup("Mon", "23", "Today", true, new List<CalendarItem>
        {
            new CalendarItem { Time = "10:00", Period = "AM", Title = "CS101 Algorithm Analysis", Subtitle = "IIT Deadline: Submit via Portal", PriorityColor = "#ef4444" },
            new CalendarItem { Time = "02:00", Period = "PM", Title = "Study Group", Subtitle = "Library Room 304 • 1h 30m", PriorityColor = "#3b82f6" }
        }));

        // Tomorrow
        Days.Add(new CalendarGroup("Tue", "24", "Tomorrow", false, new List<CalendarItem>
        {
            new CalendarItem { Time = "11:59", Period = "PM", Title = "Submit Physics Lab Report", Subtitle = "PHY202 • Late submission penalty applies", PriorityColor = "#f97316" }
        }));

        // Wednesday (Empty placeholder visual logic handled in view if needed, or item with special flag)
        // For now just items.
    }
}
