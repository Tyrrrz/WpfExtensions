using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Tyrrrz.WpfExtensions.Behaviors
{
    /// <summary>
    /// Allows two way binding on SelectedItem proprety of a TreeView
    /// </summary>
    public class TreeViewSelectionBehavior : Behavior<TreeView>
    {
        /// <summary>
        /// Delegate that checks whether one node is a child of another node
        /// <param name="nodeA">(Suspected) child node</param>
        /// <param name="nodeB">(Suspected) parent node</param>
        /// </summary>
        public delegate bool IsChildOfPredicate(object nodeA, object nodeB);

        #region Static

        /// <summary>
        /// SelectedItem dependency property
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(object),
                typeof(TreeViewSelectionBehavior),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedItemChanged));

        /// <summary>
        /// HierarchyPredicate dependency property
        /// </summary>
        public static readonly DependencyProperty HierarchyPredicateProperty =
            DependencyProperty.Register(nameof(HierarchyPredicate), typeof(IsChildOfPredicate),
                typeof(TreeViewSelectionBehavior),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// ExpandSelected dependency proprety
        /// </summary>
        public static readonly DependencyProperty ExpandSelectedProperty =
            DependencyProperty.Register(nameof(ExpandSelected), typeof(bool),
                typeof(TreeViewSelectionBehavior),
                new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Called when selected item property is changed from binding source
        /// </summary>
        private static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var behavior = (TreeViewSelectionBehavior) sender;
            if (behavior._modelHandled) return;

            if (behavior.AssociatedObject == null)
                return;

            behavior._modelHandled = true;
            behavior.UpdateAllTreeViewItems();
            behavior._modelHandled = false;
        }

        #endregion

        private readonly EventSetter _treeViewItemEventSetter;
        private bool _modelHandled;

        /// <summary>
        /// Bindable selected item
        /// </summary>
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// Predicate that checks if one node is a child of another
        /// </summary>
        public IsChildOfPredicate HierarchyPredicate
        {
            get { return (IsChildOfPredicate) GetValue(HierarchyPredicateProperty); }
            set { SetValue(HierarchyPredicateProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the selected item should also be expanded
        /// </summary>
        public bool ExpandSelected
        {
            get { return (bool) GetValue(ExpandSelectedProperty); }
            set { SetValue(ExpandSelectedProperty, value); }
        }

        /// <inheritdoc />
        public TreeViewSelectionBehavior()
        {
            _treeViewItemEventSetter = new EventSetter(
                FrameworkElement.LoadedEvent,
                new RoutedEventHandler(OnTreeViewItemLoaded));
        }

        /// <summary>
        /// Updates Selected and Expanded state of a single tree view item and (optionally) all its children recursively
        /// </summary>
        private void UpdateTreeViewItem(TreeViewItem item, bool recurse)
        {
            if (SelectedItem == null) return;
            var model = item.DataContext;

            // If the selected item is this model and is not yet selected - select and return
            if (SelectedItem == model && !item.IsSelected)
            {
                item.IsSelected = true;
                if (ExpandSelected)
                    item.IsExpanded = true;
            }
            // If the selected item is a parent of this model - expand
            else
            {
                bool isParentOfModel = HierarchyPredicate?.Invoke(SelectedItem, model) ?? true;
                if (isParentOfModel)
                    item.IsExpanded = true;
            }

            // Recurse into children
            if (recurse)
            {
                foreach (var subitem in item.Items)
                {
                    var tvi = item.ItemContainerGenerator.ContainerFromItem(subitem) as TreeViewItem;
                    if (tvi != null)
                        UpdateTreeViewItem(tvi, true);
                }
            }
        }

        /// <summary>
        /// Updates Selected and Expanded states of all items in the tree view
        /// </summary>
        private void UpdateAllTreeViewItems()
        {
            var treeView = AssociatedObject;
            foreach (var item in treeView.Items)
            {
                var tvi = treeView.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                if (tvi != null)
                    UpdateTreeViewItem(tvi, true);
            }
        }

        /// <summary>
        /// Updates the style of a tree view item by adding event triggers
        /// </summary>
        private void UpdateTreeViewItemStyle()
        {
            if (AssociatedObject.ItemContainerStyle == null)
                AssociatedObject.ItemContainerStyle = new Style(
                    typeof(TreeViewItem),
                    Application.Current.TryFindResource(typeof(TreeViewItem)) as Style);

            if (!AssociatedObject.ItemContainerStyle.Setters.Contains(_treeViewItemEventSetter))
                AssociatedObject.ItemContainerStyle.Setters.Add(_treeViewItemEventSetter);
        }

        /// <summary>
        /// Called when items in the tree view are changed
        /// </summary>
        private void OnTreeViewItemsChanged(object sender,
            NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            UpdateAllTreeViewItems();
        }

        /// <summary>
        /// Called when selected item is changed from tree view itself (from UI)
        /// </summary>
        private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (_modelHandled) return;
            if (AssociatedObject.Items.SourceCollection == null) return;
            SelectedItem = e.NewValue;
        }

        /// <summary>
        /// Called when a tree view item is loaded
        /// </summary>
        private void OnTreeViewItemLoaded(object sender, RoutedEventArgs e)
        {
            UpdateTreeViewItem((TreeViewItem) sender, false);
        }

        /// <inheritdoc />
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.SelectedItemChanged += OnTreeViewSelectedItemChanged;
            ((INotifyCollectionChanged) AssociatedObject.Items).CollectionChanged += OnTreeViewItemsChanged;

            UpdateTreeViewItemStyle();
            _modelHandled = true;
            UpdateAllTreeViewItems();
            _modelHandled = false;
        }

        /// <inheritdoc />
        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (AssociatedObject != null)
            {
                AssociatedObject.SelectedItemChanged -= OnTreeViewSelectedItemChanged;
                ((INotifyCollectionChanged) AssociatedObject.Items).CollectionChanged -= OnTreeViewItemsChanged;
            }
        }
    }
}