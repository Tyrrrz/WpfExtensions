using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Tyrrrz.WpfExtensions.Converters
{
    /// <summary>
    /// Converts bool to inverted value to visibility
    /// </summary>
    [ValueConversion(typeof (bool), typeof (Visibility))]
    public class InvertBoolToVisibilityConverter : BoolToVisibilityConverter
    {
        /// <inheritdoc />
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = (bool) value;
            return (Visibility) base.Convert(!flag, targetType, parameter, culture);
        }

        /// <inheritdoc />
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = (bool) base.ConvertBack(value, targetType, parameter, culture);
            return !flag;
        }
    }
}