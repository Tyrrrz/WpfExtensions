using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace Tyrrrz.WpfExtensions.Behaviors
{
    /// <summary>
    /// Allows bubbling of scroll events from child to parent
    /// </summary>
    public class ScrollBubblingBehavior : Behavior<FrameworkElement>
    {
        #region Static

        /// <summary>
        /// AlwaysBubble dependency property
        /// </summary>
        public static readonly DependencyProperty AlwaysBubbleProperty =
            DependencyProperty.Register(nameof(AlwaysBubble), typeof(bool),
                typeof(ScrollBubblingBehavior),
                new FrameworkPropertyMetadata(false));

        private static T GetVisualChild<T>(DependencyObject parent) where T : Visual
        {
            T child = null;
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                var v = (Visual) VisualTreeHelper.GetChild(parent, i);
                child = v as T ?? GetVisualChild<T>(v);
                if (child != null)
                    break;
            }
            return child;
        }

        #endregion

        /// <summary>
        /// When set to false, bubbling will only occur if the scrolling eached its limit.
        /// If set to true, bubbling will always occur.
        /// </summary>
        public bool AlwaysBubble
        {
            get { return (bool) GetValue(AlwaysBubbleProperty); }
            set { SetValue(AlwaysBubbleProperty, value); }
        }

        private void PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Determine if there's a need to bubble
            bool shouldBubble;
            if (AlwaysBubble)
            {
                shouldBubble = true;
            }
            else
            {
                var scrollViewer = AssociatedObject as ScrollViewer ?? GetVisualChild<ScrollViewer>(AssociatedObject);
                if (scrollViewer != null)
                {
                    var scrollPos = scrollViewer.ContentVerticalOffset;
                    shouldBubble = (scrollPos >= scrollViewer.ScrollableHeight && e.Delta < 0) ||
                                   (scrollPos <= 0 && e.Delta > 0);
                }
                else
                {
                    shouldBubble = true;
                }
            }
            if (!shouldBubble) return;

            // Bubble the event
            e.Handled = true;
            var bubble = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {
                RoutedEvent = UIElement.MouseWheelEvent
            };
            AssociatedObject.RaiseEvent(bubble);
        }

        /// <inheritdoc />
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.PreviewMouseWheel += PreviewMouseWheel;
        }

        /// <inheritdoc />
        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (AssociatedObject != null)
            {
                AssociatedObject.PreviewMouseWheel -= PreviewMouseWheel;
            }
        }
    }
}