using System.Globalization;

namespace Weak.Converters;

public class ProgressWidthConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double progress)
        {
            // Assuming the parent container width is available
            // We'll use a base width that works well on mobile devices
            // This will be percentage-based (0-1 becomes 0-100%)
            return progress * 100; // Will be interpreted as percentage in binding
        }
        return 0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
