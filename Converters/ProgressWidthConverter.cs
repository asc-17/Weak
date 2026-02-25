using System.Globalization;

namespace Weak.Converters;

public class ProgressWidthConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int percent)
        {
            return percent / 100.0;
        }
        if (value is double dPercent)
        {
            return dPercent / 100.0;
        }
        return 0.0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
