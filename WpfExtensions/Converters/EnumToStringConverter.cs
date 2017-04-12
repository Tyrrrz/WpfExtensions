using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace Tyrrrz.WpfExtensions.Converters
{
    /// <summary>
    /// Converts an enum value to <see cref="string"/>
    /// </summary>
    [ValueConversion(typeof(Enum), typeof(string))]
    public class EnumToStringConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var e = (Enum) value;
            string str = e.ToString();
            return Regex.Replace(str, @"([a-z])([A-Z])", "$1 $2");
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            string str = (string) value;
            str = str.Replace(" ", "");
            return Enum.Parse(targetType, str, true);
        }
    }
}