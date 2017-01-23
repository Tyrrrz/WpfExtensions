using System;
using System.Globalization;
using System.Windows.Data;

namespace Tyrrrz.WpfExtensions.Converters
{
    /// <summary>
    /// Converts bool to inverted bool value
    /// </summary>
    [ValueConversion(typeof (bool), typeof (bool))]
    public class InvertBoolConverter : IValueConverter
    {
        /// <summary>
        /// Inverses bool
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = (bool) value;
            return !flag;
        }

        /// <summary>
        /// Inverses bool
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = (bool) value;
            return !flag;
        }
    }
}