using System.Globalization;

namespace AlarmBle.Converters;

public class ByteToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var result = BitConverter.ToBoolean(value as byte[], 0);
        return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? new byte[1] { 0x01 } : new byte[1] { 0x00 };
    }
}

