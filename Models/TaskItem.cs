using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace Weak.Models;

public class TaskItem : ObservableObject
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    private string _title = string.Empty;
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    private DateTime _deadline;
    public DateTime Deadline
    {
        get => _deadline;
        set
        {
            if (SetProperty(ref _deadline, value))
            {
                OnPropertyChanged(nameof(DeadlineText));
                OnPropertyChanged(nameof(DeadlineColor));
            }
        }
    }

    private double _completionPercent;
    public double CompletionPercent
    {
        get => _completionPercent;
        set
        {
            if (SetProperty(ref _completionPercent, value))
            {
                OnPropertyChanged(nameof(IsCompleted));
                OnPropertyChanged(nameof(DeadlineText));
                OnPropertyChanged(nameof(DeadlineColor));
            }
        }
    }

    private int _effort = 5;
    public int Effort
    {
        get => _effort;
        set => SetProperty(ref _effort, value);
    }

    private string? _category;
    public string? Category
    {
        get => _category;
        set => SetProperty(ref _category, value);
    }

    private string? _subject;
    public string? Subject
    {
        get => _subject;
        set => SetProperty(ref _subject, value);
    }

    private string _subjectColor = "#64748b";
    public string SubjectColor
    {
        get => _subjectColor;
        set => SetProperty(ref _subjectColor, value);
    }

    private TaskSource _source = TaskSource.Manual;
    public TaskSource Source
    {
        get => _source;
        set => SetProperty(ref _source, value);
    }

    private string? _externalId;
    public string? ExternalId
    {
        get => _externalId;
        set => SetProperty(ref _externalId, value);
    }

    private bool _isDayOnly;
    public bool IsDayOnly
    {
        get => _isDayOnly;
        set => SetProperty(ref _isDayOnly, value);
    }

    private string _recurrenceType = "none";
    public string RecurrenceType
    {
        get => _recurrenceType;
        set
        {
            if (SetProperty(ref _recurrenceType, value))
                OnPropertyChanged(nameof(IsRecurring));
        }
    }

    private int _recurrenceInterval;
    public int RecurrenceInterval
    {
        get => _recurrenceInterval;
        set => SetProperty(ref _recurrenceInterval, value);
    }

    private int? _parentListId;
    public int? ParentListId
    {
        get => _parentListId;
        set => SetProperty(ref _parentListId, value);
    }

    private int? _recurrenceParentId;
    public int? RecurrenceParentId
    {
        get => _recurrenceParentId;
        set => SetProperty(ref _recurrenceParentId, value);
    }

    private DateTime _createdAt = DateTime.UtcNow;
    public DateTime CreatedAt
    {
        get => _createdAt;
        set => SetProperty(ref _createdAt, value);
    }

    private bool _isExpanded;
    [Ignore]
    public bool IsExpanded
    {
        get => _isExpanded;
        set => SetProperty(ref _isExpanded, value);
    }

    [Ignore]
    public bool IsRecurring => RecurrenceType != "none";

    [Ignore]
    public bool IsPending => Deadline.Date < DateTime.Today && CompletionPercent < 100;

    [Ignore]
    public bool IsCompleted
    {
        get => CompletionPercent >= 100;
        set => CompletionPercent = value ? 100 : 0;
    }

    [Ignore]
    public string DeadlineText
    {
        get
        {
            if (IsCompleted)
                return "Done";
            
            var today = DateTime.Today;
            if (Deadline.Date == today)
                return "Today";
            
            if (Deadline.Date == today.AddDays(1))
                return "Tomorrow";
            
            return Deadline.ToString("ddd, MMM dd");
        }
    }

    [Ignore]
    public string DeadlineColor
    {
        get
        {
            if (IsCompleted)
                return "#9ca3af";
            
            var today = DateTime.Today;
            if (Deadline.Date < today)
                return "#ef4444";
            
            if (Deadline.Date == today)
                return "#f59e0b";
            
            return "#64748b";
        }
    }

    [Ignore]
    public string EffortBarColor
    {
        get
        {
            // Low effort (1-3): Muted green
            if (Effort <= 3)
                return "#81C784";
            // Medium-low effort (4-6): Muted yellow/beige
            else if (Effort <= 6)
                return "#FFB74D";
            // Medium-high effort (7-8): Muted orange
            else if (Effort <= 8)
                return "#FF8A65";
            // High effort (9-10): Muted red
            else
                return "#E57373";
        }
    }
}

public enum TaskSource
{
    Manual,
    Calendar
}

