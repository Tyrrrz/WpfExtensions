using System;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace Tyrrrz.WpfExtensions.Converters
{
    /// <summary>
    /// Converts byte array to string
    /// </summary>
    [ValueConversion(typeof(byte[]), typeof(string))]
    public class ByteArrayToStringConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            var bytes = (byte[]) value;
            var encoding = parameter as Encoding ?? Encoding.UTF8;
            return encoding.GetString(bytes);
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            string str = (string) value;
            var encoding = parameter as Encoding ?? Encoding.UTF8;
            return encoding.GetBytes(str);
        }
    }
}