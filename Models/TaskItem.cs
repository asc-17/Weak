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
}
