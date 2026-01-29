using CommunityToolkit.Mvvm.ComponentModel;

namespace Weak.Models;

public partial class Subject : ObservableObject
{
    [ObservableProperty]
    private string name;

    [ObservableProperty]
    private string icon;

    [ObservableProperty]
    private string color;

    public string Initial => !string.IsNullOrEmpty(Name) ? Name[0].ToString().ToUpper() : "?";
}
