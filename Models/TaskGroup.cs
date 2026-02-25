using System.Collections.ObjectModel;

namespace Weak.Models;

public class TaskGroup : ObservableCollection<TaskItem>
{
    public string Header { get; }
    public DateTime WeekStart { get; }

    public TaskGroup(string header, DateTime weekStart, IEnumerable<TaskItem> items) : base(items)
    {
        Header = header;
        WeekStart = weekStart;
    }
}
