using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Tyrrrz.WpfExtensions.Converters
{
    /// <summary>
    /// Converts bool to visibility
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Parses the input string as a visibility enum and returns the result or default value if parsing failed
        /// </summary>
        protected static Visibility ParseOrDefault(string str, Visibility defaultVisibility)
        {
            var result = defaultVisibility;
            if (string.IsNullOrWhiteSpace(str)) return result;
            try
            {
                result = (Visibility) Enum.Parse(typeof (Visibility), str);
            }
            catch
            {
                // Ignored
            }
            return result;
        }

        /// <summary>
        /// Converts bool to visibility
        /// </summary>
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visibility = Visibility.Hidden;
            bool flag = (bool) value;

            // Set default visibility if parameter is set
            if (parameter is Visibility)
                visibility = (Visibility) parameter;
            else
                visibility = ParseOrDefault(parameter as string, visibility);

            return flag ? Visibility.Visible : visibility;
        }

        /// <summary>
        /// Converts visibility to bool
        /// </summary>
        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visibility = (Visibility) value;
            return visibility == Visibility.Visible;
        }
    }
}