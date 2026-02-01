using System.Globalization;

namespace Weak.Converters;

public class ProgressWidthConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int percent)
        {
            // Return the percentage value which will be used with Grid binding
            // For a 100% progress, we want it to fill the entire parent
            // The parent Grid column is set to "*" which takes available space
            // We'll return a multiplier that represents the percentage
            return percent / 100.0; // 100 becomes 1.0, 50 becomes 0.5, etc.
        }
        return 0.0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
