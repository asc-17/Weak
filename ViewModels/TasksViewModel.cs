using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Weak.Models;

namespace Weak.ViewModels;

public partial class TasksViewModel : ObservableObject
{
    public ObservableCollection<TaskItem> Tasks { get; } = new();
    public ObservableCollection<Subject> Subjects { get; } = new();

    [ObservableProperty]
    private string currentDate;

    public int TaskCount => Tasks.Count;

    public TasksViewModel()
    {
        CurrentDate = DateTime.Now.ToString("dddd, MMM dd");
        LoadMockData();
    }

    private void LoadMockData()
    {
        Tasks.Clear();
        
        // Tasks from the Stitch design
        Tasks.Add(new TaskItem 
        { 
            Title = "Read Chapter 4: Macroeconomics", 
            Deadline = DateTime.Today, 
            SubjectColor = "#ef4444",
            IsCompleted = false
        });
        
        Tasks.Add(new TaskItem 
        { 
            Title = "Submit Calculus Problem Set", 
            Deadline = DateTime.Today.AddDays(-1), 
            SubjectColor = "#3b82f6",
            IsCompleted = true
        });
        
        Tasks.Add(new TaskItem 
        { 
            Title = "Draft History Essay", 
            Deadline = DateTime.Today.AddHours(17), // Fri, 5:00 PM
            SubjectColor = "#eab308",
            IsCompleted = false
        });
        
        Tasks.Add(new TaskItem 
        { 
            Title = "Lab Report: Physics", 
            Deadline = DateTime.Today.AddDays(3).AddHours(9), // Mon, 9:00 AM
            SubjectColor = "#8b5cf6",
            IsCompleted = false
        });

        Subjects.Clear();
        
        // Subjects with emoji icons
        Subjects.Add(new Subject 
        { 
            Name = "Economics", 
            Icon = "???",
            IconBackground = "#dbeafe",
            IconColor = "#1d4ed8"
        });
        
        Subjects.Add(new Subject 
        { 
            Name = "Calculus II", 
            Icon = "?",
            IconBackground = "#e0e7ff",
            IconColor = "#4f46e5"
        });
        
        Subjects.Add(new Subject 
        { 
            Name = "World History", 
            Icon = "???",
            IconBackground = "#fef3c7",
            IconColor = "#d97706"
        });
        
        Subjects.Add(new Subject 
        { 
            Name = "Physics", 
            Icon = "??",
            IconBackground = "#ddd6fe",
            IconColor = "#7c3aed"
        });
    }

    [RelayCommand]
    private void ToggleTask(TaskItem task)
    {
        if (task != null)
        {
            task.IsCompleted = !task.IsCompleted;
        }
    }

    [RelayCommand]
    private void AddTask()
    {
        // Stub for adding new task
    }
}

