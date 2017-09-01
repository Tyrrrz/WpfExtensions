using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Tyrrrz.WpfExtensions.Converters
{
    /// <summary>
    /// Converts <see cref="IEnumerable"/> to <see cref="string"/> by using <see cref="string.Join(string,object[])"/>
    /// </summary>
    [ValueConversion(typeof(IEnumerable), typeof(string))]
    public class ArrayToStringConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            var enumerable = (IEnumerable) value;
            var separator = parameter as string ?? ", ";
            return string.Join(separator, enumerable.Cast<object>());
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            var joined = (string) value;
            var separator = parameter as string ?? ", ";
            return joined.Split(new [] {separator}, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}