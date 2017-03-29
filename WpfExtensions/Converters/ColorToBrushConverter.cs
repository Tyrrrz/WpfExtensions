using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Tyrrrz.WpfExtensions.Converters
{
    /// <summary>
    /// Converts color to solid color brush
    /// </summary>
    [ValueConversion(typeof (Color), typeof (Brush))]
    public class ColorToBrushConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var color = (Color) value;
            return new SolidColorBrush(color);
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var c = (SolidColorBrush) value;
            return c.Color;
        }
    }
}