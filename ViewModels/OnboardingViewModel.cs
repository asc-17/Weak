using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Weak.Models;
using Weak.Services;

namespace Weak.ViewModels;

public partial class OnboardingViewModel : ObservableObject
{
    private readonly SettingsService _settingsService;

    public ObservableCollection<OnboardingStep> Steps { get; } = new();

    [ObservableProperty]
    private int currentIndex;

    [ObservableProperty]
    private OnboardingStep? currentStep;

    [ObservableProperty]
    private string userName = string.Empty;

    [ObservableProperty]
    private TimeSpan wakeTime = TimeSpan.FromHours(7);

    [ObservableProperty]
    private TimeSpan sleepTime = TimeSpan.FromHours(22);

    [ObservableProperty]
    private string weekStartDay = "sunday";

    public bool IsFirstStep => CurrentIndex == 0;
    public bool IsLastStep => Steps.Count > 0 && CurrentIndex == Steps.Count - 1;
    public bool IsGoogleStep => CurrentIndex == 4;
    public bool IsSundaySelected => WeekStartDay == "sunday";
    public bool IsMondaySelected => WeekStartDay == "monday";
    public string NextButtonText => IsLastStep ? "Get Started" : "Next";

    public OnboardingViewModel(SettingsService settingsService)
    {
        _settingsService = settingsService;
        InitializeSteps();
    }

    partial void OnCurrentIndexChanged(int value)
    {
        OnPropertyChanged(nameof(IsFirstStep));
        OnPropertyChanged(nameof(IsLastStep));
        OnPropertyChanged(nameof(IsGoogleStep));
        OnPropertyChanged(nameof(NextButtonText));
    }

    partial void OnWeekStartDayChanged(string value)
    {
        OnPropertyChanged(nameof(IsSundaySelected));
        OnPropertyChanged(nameof(IsMondaySelected));
    }

    private void InitializeSteps()
    {
        Steps.Add(new OnboardingStep { StepType = "welcome", Title = "Meet Weak", Subtitle = "Your weekly workload, at a glance.", IsActive = true });
        Steps.Add(new OnboardingStep { StepType = "name", Title = "What should we call you?", Subtitle = "" });
        Steps.Add(new OnboardingStep { StepType = "times", Title = "When's your day?", Subtitle = "We'll use these in your Day timeline." });
        Steps.Add(new OnboardingStep { StepType = "weekstart", Title = "When does your week start?", Subtitle = "" });
        Steps.Add(new OnboardingStep { StepType = "google", Title = "Connect Google Calendar", Subtitle = "Auto-import events. You can skip this." });
        Steps.Add(new OnboardingStep { StepType = "done", Title = "You're all set.", Subtitle = "Let's see your workload." });

        CurrentStep = Steps[0];
    }

    [RelayCommand]
    private void Next()
    {
        if (IsLastStep)
        {
            _ = FinishAsync();
            return;
        }
        NavigateToIndex(CurrentIndex + 1);
    }

    [RelayCommand]
    private void Back()
    {
        if (IsFirstStep) return;
        NavigateToIndex(CurrentIndex - 1);
    }

    [RelayCommand]
    private void Skip()
    {
        NavigateToIndex(Steps.Count - 1);
    }

    [RelayCommand]
    private void SetWeekStart(string day)
    {
        WeekStartDay = day;
    }

    [RelayCommand]
    private async Task ConnectGoogle()
    {
        await Application.Current!.MainPage!.DisplayAlert(
            "Coming Soon",
            "Google Calendar integration will be available in a future update.",
            "OK");
    }

    [RelayCommand]
    private async Task Finish()
    {
        await FinishAsync();
    }

    private async Task FinishAsync()
    {
        var settings = await _settingsService.GetSettingsAsync();
        settings.Name = UserName.Trim();
        settings.WakeTime = WakeTime;
        settings.SleepTime = SleepTime;
        settings.WeekStartDay = WeekStartDay;
        settings.OnboardingComplete = true;
        await _settingsService.SaveSettingsAsync(settings);

        Application.Current!.MainPage = new AppShell();
    }

    private void NavigateToIndex(int index)
    {
        if (index < 0 || index >= Steps.Count) return;
        Steps[CurrentIndex].IsActive = false;
        CurrentIndex = index;
        Steps[CurrentIndex].IsActive = true;
        CurrentStep = Steps[CurrentIndex];
    }
}
