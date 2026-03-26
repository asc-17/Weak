using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Weak.Models;
using Weak.Services;

namespace Weak.ViewModels;

[QueryProperty(nameof(ListId), "listId")]
public partial class ListViewModel : ObservableObject
{
    private readonly TaskListRepository _taskListRepository;
    private readonly TaskRepository _taskRepository;

    [ObservableProperty]
    private int listId;

    [ObservableProperty]
    private string listName = string.Empty;

    [ObservableProperty]
    private DateTime dueDate = DateTime.Today;

    [ObservableProperty]
    private double weightedCompletionPercent;

    [ObservableProperty]
    private double averageEffort;

    [ObservableProperty]
    private string subtaskProgress = string.Empty;

    [ObservableProperty]
    private bool isAddingSubtask;

    [ObservableProperty]
    private string newSubtaskName = string.Empty;

    [ObservableProperty]
    private double newSubtaskEffort = 5;

    [ObservableProperty]
    private DateTime newSubtaskDate = DateTime.Today;

    public ObservableCollection<TaskItem> Subtasks { get; } = new();

    private TaskList? _currentList;

    public ListViewModel(TaskListRepository taskListRepository, TaskRepository taskRepository)
    {
        _taskListRepository = taskListRepository;
        _taskRepository = taskRepository;
    }

    public async Task InitializeAsync()
    {
        _currentList = await _taskListRepository.GetTaskListByIdAsync(ListId);
        if (_currentList == null) return;

        ListName = _currentList.Name;
        DueDate = _currentList.DueDate;

        await LoadSubtasksAsync();
    }

    private async Task LoadSubtasksAsync()
    {
        var subtasks = await _taskListRepository.GetSubtasksAsync(ListId);
        Subtasks.Clear();
        foreach (var subtask in subtasks)
            Subtasks.Add(subtask);

        if (_currentList != null)
        {
            _currentList.Subtasks = subtasks;
            _taskListRepository.RecalculateListPropertiesFromSubtasks(_currentList);
            WeightedCompletionPercent = _currentList.WeightedCompletionPercent;
            AverageEffort = _currentList.AverageEffort;
            SubtaskProgress = _currentList.SubtaskProgress;
        }
    }

    [RelayCommand]
    private async Task ToggleSubtask(TaskItem subtask)
    {
        if (subtask == null) return;

        subtask.IsCompleted = !subtask.IsCompleted;
        await _taskRepository.SaveTaskAsync(subtask);
        await LoadSubtasksAsync();
    }

    [RelayCommand]
    private void ShowAddSubtask()
    {
        IsAddingSubtask = true;
        NewSubtaskName = string.Empty;
        NewSubtaskEffort = 5;
        NewSubtaskDate = DueDate;
    }

    [RelayCommand]
    private void CancelAddSubtask()
    {
        IsAddingSubtask = false;
    }

    [RelayCommand]
    private async Task AddSubtask()
    {
        if (string.IsNullOrWhiteSpace(NewSubtaskName)) return;

        var subtask = new TaskItem
        {
            Title = NewSubtaskName,
            Effort = (int)Math.Round(NewSubtaskEffort),
            Deadline = NewSubtaskDate,
            CompletionPercent = 0,
            ParentListId = ListId,
            Source = TaskSource.Manual
        };

        await _taskRepository.SaveTaskAsync(subtask);
        IsAddingSubtask = false;
        await LoadSubtasksAsync();
    }

    [RelayCommand]
    private async Task DeleteSubtask(TaskItem subtask)
    {
        if (subtask == null) return;

        await _taskRepository.DeleteTaskAsync(subtask);
        await LoadSubtasksAsync();
    }

    [RelayCommand]
    private async Task SaveListDetails()
    {
        if (_currentList == null) return;

        _currentList.Name = ListName;
        _currentList.DueDate = DueDate;

        await _taskListRepository.SaveTaskListAsync(_currentList);
    }

    [RelayCommand]
    private async Task GoBack()
    {
        await SaveListDetails();
        await Shell.Current.GoToAsync("..");
    }
}
