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
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var flag = (bool) value;
            // ReSharper disable once PossibleNullReferenceException
            return (Visibility) base.Convert(!flag, targetType, parameter, culture);
        }

        /// <inheritdoc />
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            // ReSharper disable once PossibleNullReferenceException
            var flag = (bool) base.ConvertBack(value, targetType, parameter, culture);
            return !flag;
        }
    }
}