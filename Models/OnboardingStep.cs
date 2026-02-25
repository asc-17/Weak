using CommunityToolkit.Mvvm.ComponentModel;

namespace Weak.Models;

public class OnboardingStep : ObservableObject
{
    private bool _isActive;
    public bool IsActive
    {
        get => _isActive;
        set => SetProperty(ref _isActive, value);
    }

    public string StepType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
}
