using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Tyrrrz.WpfExtensions.Behaviors
{
    /// <summary>
    /// Allows two way binding of string lines of a text box
    /// </summary>
    public class TextBoxLinesBehavior : Behavior<TextBox>
    {
        #region Static
        /// <summary>
        /// Lines dependency property
        /// </summary>
        public static readonly DependencyProperty LinesProperty =
            DependencyProperty.Register(nameof(Lines), typeof(string[]), typeof(TextBoxLinesBehavior),
                new FrameworkPropertyMetadata(new string[0], FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnLinesChanged));

        /// <summary>
        /// Called when lines property is changed from binding source
        /// </summary>
        private static void OnLinesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var behavior = (TextBoxLinesBehavior) sender;
            if (behavior._modelHandled) return;

            if (behavior.AssociatedObject == null)
                return;

            behavior._modelHandled = true;
            behavior.LinesToText();
            behavior._modelHandled = false;
        }
        #endregion

        private bool _modelHandled;

        /// <summary>
        /// Bindable lines
        /// </summary>
        public string[] Lines
        {
            get { return (string[]) GetValue(LinesProperty); }
            set { SetValue(LinesProperty, value); }
        }

        private void LinesToText()
        {
            AssociatedObject.Text = string.Join(Environment.NewLine, Lines);
        }

        private void TextToLines()
        {
            Lines = AssociatedObject.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        }

        private void OnTextBoxTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            if (_modelHandled) return;
            TextToLines();
        }

        /// <inheritdoc />
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.TextChanged += OnTextBoxTextChanged;

            _modelHandled = true;
            LinesToText();
            _modelHandled = false;
        }

        /// <inheritdoc />
        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (AssociatedObject != null)
            {
                AssociatedObject.TextChanged -= OnTextBoxTextChanged;
            }
        }
    }
}
