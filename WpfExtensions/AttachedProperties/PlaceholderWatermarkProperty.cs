using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using Tyrrrz.WpfExtensions.Types;

namespace Tyrrrz.WpfExtensions.AttachedProperties
{
    /// <summary>
    /// Attached property that allows display of a watermark as a placeholder for control's content
    /// </summary>
    public static class PlaceholderWatermarkProperty
    {
        /// <summary>
        /// Watermark Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.RegisterAttached(
           "Watermark",
           typeof(object),
           typeof(PlaceholderWatermarkProperty),
           new FrameworkPropertyMetadata(null, OnWatermarkChanged));


        /// <summary>
        /// Dictionary of ItemsControls
        /// </summary>
        private static Dictionary<object, ItemsControl> ItemsControls { get; } = new Dictionary<object, ItemsControl>();

        /// <summary>
        /// Gets the Watermark property. This dependency property indicates the watermark for the control.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject"/> to get the property from</param>
        /// <returns>The value of the Watermark property</returns>
        public static object GetWatermark(DependencyObject d)
        {
            return d.GetValue(WatermarkProperty);
        }

        /// <summary>
        /// Sets the Watermark property.  This dependency property indicates the watermark for the control.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject"/> to set the property on</param>
        /// <param name="value">value of the property</param>
        public static void SetWatermark(DependencyObject d, object value)
        {
            d.SetValue(WatermarkProperty, value);
        }

        /// <summary>
        /// Handles changes to the Watermark property.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject"/> that fired the event</param>
        /// <param name="e">A <see cref="DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnWatermarkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (Control)d;
            control.Loaded += Control_Loaded;

            if (d is ComboBox || d is TextBox)
            {
                control.GotKeyboardFocus += Control_GotKeyboardFocus;
                control.LostKeyboardFocus += Control_Loaded;
            }

            var itemsControl = d as ItemsControl;
            if (itemsControl != null && !(d is ComboBox))
            {
                // for Items property  
                itemsControl.ItemContainerGenerator.ItemsChanged += ItemsChanged;
                ItemsControls.Add(itemsControl.ItemContainerGenerator, itemsControl);

                // for ItemsSource property  
                var prop = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, itemsControl.GetType());
                prop.AddValueChanged(itemsControl, ItemsSourceChanged);
            }
        }

        /// <summary>
        /// Handle the GotFocus event on the control
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="RoutedEventArgs"/> that contains the event data.</param>
        private static void Control_GotKeyboardFocus(object sender, RoutedEventArgs e)
        {
            var c = (Control) sender;
            if (ShouldShowWatermark(c))
                RemoveWatermark(c);
        }

        /// <summary>
        /// Handle the Loaded and LostFocus event on the control
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="RoutedEventArgs"/> that contains the event data.</param>
        private static void Control_Loaded(object sender, RoutedEventArgs e)
        {
            var control = (Control) sender;
            if (ShouldShowWatermark(control))
                ShowWatermark(control);
        }

        /// <summary>
        /// Event handler for the items source changed event
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="EventArgs"/> that contains the event data.</param>
        private static void ItemsSourceChanged(object sender, EventArgs e)
        {
            var c = (ItemsControl) sender;
            if (c.ItemsSource != null)
            {
                if (ShouldShowWatermark(c))
                    ShowWatermark(c);
                else
                    RemoveWatermark(c);
            }
            else
            {
                ShowWatermark(c);
            }
        }

        /// <summary>
        /// Event handler for the items changed event
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="ItemsChangedEventArgs"/> that contains the event data.</param>
        private static void ItemsChanged(object sender, ItemsChangedEventArgs e)
        {
            ItemsControl control;
            if (!ItemsControls.TryGetValue(sender, out control)) return;

            if (ShouldShowWatermark(control))
                ShowWatermark(control);
            else
                RemoveWatermark(control);
        }


        /// <summary>
        /// Remove the watermark from the specified element
        /// </summary>
        /// <param name="control">Element to remove the watermark from</param>
        private static void RemoveWatermark(UIElement control)
        {
            var layer = AdornerLayer.GetAdornerLayer(control);

            // Get adorners
            var adorners = layer?.GetAdorners(control);
            if (adorners == null) return;

            foreach (var adorner in adorners.OfType<WatermarkAdorner>())
            {
                adorner.Visibility = Visibility.Hidden;
                layer.Remove(adorner);
            }
        }

        /// <summary>
        /// Show the watermark on the specified control
        /// </summary>
        /// <param name="control">Control to show the watermark on</param>
        private static void ShowWatermark(UIElement control)
        {
            var layer = AdornerLayer.GetAdornerLayer(control);
            layer?.Add(new WatermarkAdorner(control, GetWatermark(control)));
        }

        /// <summary>
        /// Indicates whether or not the watermark should be shown on the specified control
        /// </summary>
        /// <param name="c"><see cref="Control"/> to test</param>
        /// <returns>true if the watermark should be shown; false otherwise</returns>
        private static bool ShouldShowWatermark(Control c)
        {
            var comboBox = c as ComboBox;
            var textBox = c as TextBox;
            var itemsControl = c as ItemsControl;

            if (comboBox != null)
                return comboBox.Text == string.Empty;
            if (textBox != null)
                return textBox.Text == string.Empty;
            return itemsControl?.Items.Count == 0;
        }
    }
}