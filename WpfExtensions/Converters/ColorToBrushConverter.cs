using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Tyrrrz.WpfExtensions.Converters
{
    /// <summary>
    /// Converts color to solid color brush
    /// </summary>
    [ValueConversion(typeof (bool), typeof (Brush))]
    public class ColorToBrushConverter : IValueConverter
    {
        /// <summary>
        /// Converts color to solid color brush
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = (Color) value;
            return new SolidColorBrush(color);
        }

        /// <summary>
        /// Converts solid color brush to color
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var c = (SolidColorBrush) value;
            return c.Color;
        }
    }
}