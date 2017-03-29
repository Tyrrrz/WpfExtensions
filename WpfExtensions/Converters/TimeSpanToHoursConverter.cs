using System;
using System.Globalization;
using System.Windows.Data;

namespace Tyrrrz.WpfExtensions.Converters
{
    /// <summary>
    /// Converts timespan to hours
    /// </summary>
    [ValueConversion(typeof(TimeSpan), typeof(double))]
    public class TimeSpanToHoursConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var ts = (TimeSpan) value;
            return ts.TotalHours;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var ticks = (double) value;
            return TimeSpan.FromHours(ticks);
        }
    }
}