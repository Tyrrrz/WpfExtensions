using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace Tyrrrz.WpfExtensions.Converters
{
    /// <summary>
    /// Converts <see cref="IEnumerable"/> to <see cref="string"/> by using <see cref="string.Join(string,object[])"/>
    /// </summary>
    [ValueConversion(typeof(IEnumerable), typeof(string))]
    public class StringJoinConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var enumerable = (IEnumerable) value;
            string separator = (string) parameter ?? ", ";
            return string.Join(separator, enumerable);
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string joined = (string) value;
            string separator = (string) parameter ?? ", ";
            return joined.Split(new [] {separator}, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}