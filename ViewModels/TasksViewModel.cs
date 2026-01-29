using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Weak.Models;

namespace Weak.ViewModels;

public partial class TasksViewModel : ObservableObject
{
    public ObservableCollection<TaskItem> Tasks { get; } = new();
    public ObservableCollection<Subject> Subjects { get; } = new();

    public TasksViewModel()
    {
        LoadMockData();
    }

    private void LoadMockData()
    {
        Tasks.Clear();
        Tasks.Add(new TaskItem { Title = "Calculus Problem Set 4", Deadline = DateTime.Now.AddDays(1), SubjectColor = "#ef4444" }); // Red
        Tasks.Add(new TaskItem { Title = "History Essay Draft", Deadline = DateTime.Now.AddDays(3), SubjectColor = "#eab308" }); // Yellow
        Tasks.Add(new TaskItem { Title = "Physics Lab Report", Deadline = DateTime.Now.AddDays(2), SubjectColor = "#3b82f6" }); // Blue
        Tasks.Add(new TaskItem { Title = "Read Chapter 5-6", Deadline = DateTime.Now.AddDays(5), SubjectColor = "#22c55e" }); // Green

        Subjects.Clear();
        Subjects.Add(new Subject { Name = "Mathematics", Icon = "functions", Color = "#ef4444" });
        Subjects.Add(new Subject { Name = "Physics", Icon = "science", Color = "#3b82f6" });
        Subjects.Add(new Subject { Name = "History", Icon = "history_edu", Color = "#eab308" });
    }

    [RelayCommand]
    private void AddTask()
    {
        // Stub for FAB action
    }
}
