using System.Globalization;

namespace Weak.Converters;

public class BoolToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isTrue && parameter is string param)
        {
            if (param == "primary")
            {
                // Return primary color if true, gray if false
                return isTrue ? Color.FromArgb("#135bec") : Color.FromArgb("#f1f5f9");
            }
            else if (param == "text")
            {
                // Return white text if true, gray text if false
                return isTrue ? Colors.White : Color.FromArgb("#64748b");
            }
            else if (param == "selectedday")
            {
                // Return light blue background if selected, white if not
                return isTrue ? Color.FromArgb("#e3f2fd") : Colors.White;
            }
        }
        return Colors.Transparent;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
