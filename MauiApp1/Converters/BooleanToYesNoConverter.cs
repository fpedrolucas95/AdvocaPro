using System.Globalization;

namespace AdvocaPro.Converters
{
    public class BooleanToYesNoConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? "Sim" : "Não";
            }
            return "N/A";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                return stringValue.Equals("Sim", StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }
    }
}