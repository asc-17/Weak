using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace Weak.Models;

public class TaskList : ObservableObject
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    private string? _subject;
    public string? Subject
    {
        get => _subject;
        set => SetProperty(ref _subject, value);
    }

    private DateTime _dueDate;
    public DateTime DueDate
    {
        get => _dueDate;
        set => SetProperty(ref _dueDate, value);
    }

    private DateTime _createdAt = DateTime.UtcNow;
    public DateTime CreatedAt
    {
        get => _createdAt;
        set => SetProperty(ref _createdAt, value);
    }

    [Ignore]
    public double AverageEffort { get; set; }

    [Ignore]
    public int SubtaskCount { get; set; }

    [Ignore]
    public int CompletedSubtaskCount { get; set; }

    [Ignore]
    public string SubtaskProgress => $"{CompletedSubtaskCount}/{SubtaskCount} done";

    private bool _isExpanded;
    [Ignore]
    public bool IsExpanded
    {
        get => _isExpanded;
        set => SetProperty(ref _isExpanded, value);
    }

    [Ignore]
    public List<TaskItem> Subtasks { get; set; } = new();
}
