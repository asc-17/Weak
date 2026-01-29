using CommunityToolkit.Mvvm.ComponentModel;

namespace Weak.Models;

public partial class Subject : ObservableObject
{
    [ObservableProperty]
    private string name;

    [ObservableProperty]
    private string icon; // Unicode character for icon

    [ObservableProperty]
    private string color; // For backward compatibility

    [ObservableProperty]
    private string iconBackground = "#e0f2fe"; // Light blue background

    [ObservableProperty]
    private string iconColor = "#0284c7"; // Blue icon color

    public string Initial => !string.IsNullOrEmpty(Name) ? Name[0].ToString().ToUpper() : "?";
}

