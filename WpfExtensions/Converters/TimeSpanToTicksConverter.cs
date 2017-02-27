using System;
using System.Globalization;
using System.Windows.Data;

namespace Tyrrrz.WpfExtensions.Converters
{
    /// <summary>
    /// Converts timespan to ticks
    /// </summary>
    [ValueConversion(typeof(TimeSpan), typeof(long))]
    public class TimeSpanToTicksConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ts = (TimeSpan) value;
            return ts.Ticks;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long ticks = (long) value;
            return TimeSpan.FromTicks(ticks);
        }
    }
}