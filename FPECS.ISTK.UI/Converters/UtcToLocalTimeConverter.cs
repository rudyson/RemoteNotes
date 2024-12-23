using System.Globalization;
using System.Windows.Data;

namespace FPECS.ISTK.UI.Converters;
public class UtcToLocalTimeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DateTime utcDateTime)
        {
            var localTime = utcDateTime.ToLocalTime();
            return localTime.ToString("dd.MM.yyyy 'at' HH:mm");
      
        }

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}