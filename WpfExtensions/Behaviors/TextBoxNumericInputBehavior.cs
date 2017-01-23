using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Tyrrrz.WpfExtensions.Behaviors
{
    /// <summary>
    /// Handles input and binding for textbox, intended for numeric input
    /// </summary>
    public class TextBoxNumericInputBehavior : Behavior<TextBox>
    {
        #region Static

        private static readonly Regex FloatValidationRegex =
            new Regex(@"[^0-9\-\+" + @"\" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + @"]+",
                RegexOptions.Compiled);

        private static readonly Regex IntegerValidationRegex =
            new Regex(@"[^0-9\-\+]+",
                RegexOptions.Compiled);

        /// <summary>
        /// IsFloatingPoint dependency property
        /// </summary>
        public static readonly DependencyProperty IsFloatingPointProperty =
            DependencyProperty.Register(nameof(IsFloatingPoint), typeof(bool),
                typeof(TextBoxNumericInputBehavior),
                new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Value (double) dependency property
        /// </summary>
        public static readonly DependencyProperty ValueDoubleProperty =
            DependencyProperty.Register(nameof(ValueDouble), typeof(double),
                typeof(TextBoxNumericInputBehavior),
                new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnValueChanged));

        /// <summary>
        /// Value (int) dependency property
        /// </summary>
        public static readonly DependencyProperty ValueIntProperty =
            DependencyProperty.Register(nameof(ValueInt), typeof(long),
                typeof(TextBoxNumericInputBehavior),
                new FrameworkPropertyMetadata(default(long), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnValueChanged));

        /// <summary>
        /// Called when the value property is changed from binding source
        /// </summary>
        private static void OnValueChanged(DependencyObject sender,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var behavior = (TextBoxNumericInputBehavior) sender;
            if (behavior._modelHandled) return;

            if (behavior.AssociatedObject == null)
                return;

            behavior._modelHandled = true;
            behavior.ValueToText();
            behavior._modelHandled = false;
        }

        #endregion

        private bool _modelHandled;

        /// <summary>
        /// Gets or sets whether the number in the textbox is a floating point number.
        /// When set to true, use <see cref="ValueDouble"/>.
        /// When set to false, use <see cref="ValueInt"/>.
        /// </summary>
        public bool IsFloatingPoint
        {
            get { return (bool) GetValue(IsFloatingPointProperty); }
            set { SetValue(IsFloatingPointProperty, value); }
        }

        /// <summary>
        /// Bindable numeric value of the textbox
        /// </summary>
        public double ValueDouble
        {
            get { return (double) GetValue(ValueDoubleProperty); }
            set { SetValue(ValueDoubleProperty, value); }
        }

        /// <summary>
        /// Bindable numeric value of the textbox
        /// </summary>
        public long ValueInt
        {
            get { return (long) GetValue(ValueIntProperty); }
            set { SetValue(ValueIntProperty, value); }
        }

        private void TextToValue()
        {
            // Empty text - do nothing
            if (string.IsNullOrWhiteSpace(AssociatedObject.Text))
                return;

            // If ending with a decimal point - do nothing
            if (AssociatedObject.Text.EndsWith(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator))
                return;

            // If there is a decimal point and ending with zero - do nothing
            if (AssociatedObject.Text.Contains(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator) &&
                AssociatedObject.Text.EndsWith("0"))
                return;

            // Validate text and set
            if (IsFloatingPoint)
            {
                double value;
                if (double.TryParse(AssociatedObject.Text, out value))
                    ValueDouble = value;
            }
            else
            {
                long value;
                if (long.TryParse(AssociatedObject.Text, out value))
                    ValueInt = value;
            }
        }

        private void ValueToText()
        {
            AssociatedObject.Text = IsFloatingPoint
                ? ValueDouble.ToString(CultureInfo.CurrentCulture)
                : ValueInt.ToString(CultureInfo.CurrentCulture);
        }

        private void OnTextBoxPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Only numbers and approved symbols accepted
            e.Handled = IsFloatingPoint
                ? FloatValidationRegex.IsMatch(e.Text)
                : IntegerValidationRegex.IsMatch(e.Text);
        }

        private void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_modelHandled) return;
            TextToValue();
        }

        private void OnTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            ValueToText();
        }

        /// <inheritdoc />
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.PreviewTextInput += OnTextBoxPreviewTextInput;
            AssociatedObject.TextChanged += OnTextBoxTextChanged;
            AssociatedObject.LostFocus += OnTextBoxLostFocus;

            _modelHandled = true;
            ValueToText();
            _modelHandled = false;
        }

        /// <inheritdoc />
        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (AssociatedObject != null)
            {
                AssociatedObject.PreviewTextInput -= OnTextBoxPreviewTextInput;
                AssociatedObject.TextChanged -= OnTextBoxTextChanged;
                AssociatedObject.LostFocus -= OnTextBoxLostFocus;
            }
        }
    }
}