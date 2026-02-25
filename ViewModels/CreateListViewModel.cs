using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Weak.Models;
using Weak.Services;

namespace Weak.ViewModels;

public partial class CreateListViewModel : ObservableObject
{
    private readonly TaskListRepository _taskListRepository;

    [ObservableProperty]
    private string listName = string.Empty;

    [ObservableProperty]
    private DateTime dueDate = DateTime.Today.AddDays(7);

    [ObservableProperty]
    private string subject = string.Empty;

    public CreateListViewModel(TaskListRepository taskListRepository)
    {
        _taskListRepository = taskListRepository;
    }

    [RelayCommand]
    private async Task GoBack() => await Shell.Current.GoToAsync("..");

    [RelayCommand]
    private async Task CreateList()
    {
        if (string.IsNullOrWhiteSpace(listName))
        {
            await Application.Current!.MainPage!.DisplayAlert(
                "Validation Error", "Please enter a list name.", "OK");
            return;
        }

        var list = new TaskList
        {
            Name = listName,
            Subject = string.IsNullOrWhiteSpace(subject) ? null : subject,
            DueDate = dueDate,
            CreatedAt = DateTime.UtcNow
        };

        await _taskListRepository.SaveTaskListAsync(list);
        await Shell.Current.GoToAsync("..");
    }
}
