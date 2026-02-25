using System.Globalization;

namespace Weak.Converters;

public class IntensityBgColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var intensity = value as string;
        return intensity switch
        {
            "Low" => Color.FromArgb("#dcfce7"),
            "Moderate" => Color.FromArgb("#fef9c3"),
            "High" => Color.FromArgb("#ffedd5"),
            "Critical" => Color.FromArgb("#fee2e2"),
            _ => Color.FromArgb("#ecf2ff")
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class IntensityTextColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var intensity = value as string;
        return intensity switch
        {
            "Low" => Color.FromArgb("#15803d"),
            "Moderate" => Color.FromArgb("#854d0e"),
            "High" => Color.FromArgb("#c2410c"),
            "Critical" => Color.FromArgb("#b91c1c"),
            _ => Color.FromArgb("#104EB0")
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
