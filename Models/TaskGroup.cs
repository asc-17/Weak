using System.Collections.ObjectModel;

namespace Weak.Models;

public class TaskGroup : ObservableCollection<object>
{
    public string Header { get; }
    public DateTime WeekStart { get; }

    public TaskGroup(string header, DateTime weekStart, IEnumerable<object> items) : base(items)
    {
        Header = header;
        WeekStart = weekStart;
    }
}
