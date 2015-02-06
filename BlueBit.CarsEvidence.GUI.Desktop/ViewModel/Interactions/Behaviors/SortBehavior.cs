using BlueBit.CarsEvidence.Commons.Helpers;
using dotNetExt;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Interactions.Behaviors
{
    public static class SortBase
    {
        public static void ApplySort(ICollectionView view, string propertyNames, bool forClick)
        {
            if (string.IsNullOrEmpty(propertyNames)) return;
            var pn = propertyNames.Split(',');

            var direction = ListSortDirection.Ascending;
            if (forClick)
            {
                Contract.Assert(pn.Length == 1);
                if (view.SortDescriptions.Count > 0)
                {
                    var currentSort = view.SortDescriptions[0];
                    if (currentSort.PropertyName == pn[0])
                    {
                        if (currentSort.Direction == ListSortDirection.Ascending)
                            direction = ListSortDirection.Descending;
                        else
                            direction = ListSortDirection.Ascending;
                    }
                }
            }
            view.SortDescriptions.Clear();
            pn.Each(_ => view.SortDescriptions.Add(new SortDescription(_, direction)));
        }
    }

    public class SortDefaultBehavior :
        Behavior<ItemsControl>
    {
        public static readonly DependencyProperty PropertyNamesProperty = BehaviorBase
            .ForType<SortDefaultBehavior>.RegisterProperty(_ => _.PropertyNames);

        public string PropertyNames
        {
            get { return (string)GetValue(PropertyNamesProperty); }
            set { SetValue(PropertyNamesProperty, value); }
        }
    
        protected override void OnAttached()
        {
            base.OnAttached();
            SortBase.ApplySort(AssociatedObject.Items, PropertyNames, false);
        }
    }

    public class SortColumnBehavior :
        Behavior<GridViewColumn>
    {
        public static readonly DependencyProperty PropertyNameProperty = BehaviorBase
            .ForType<SortColumnBehavior>.RegisterProperty(_ => _.PropertyName);

        public string PropertyName
        {
            get { return (string)GetValue(PropertyNameProperty); }
            set { SetValue(PropertyNameProperty, value); }
        }
    }

    public class SortColumnsBehavior :
        Behavior<ListView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(OnClick));
        }

        private static void OnClick(object sender, RoutedEventArgs e)
        {
            var header = e.OriginalSource as GridViewColumnHeader;
            if (header == null) return;
            var propertyName = header.Column.GetBehavior<SortColumnBehavior>().GetSafeValue(_ => _.PropertyName);
            if (string.IsNullOrEmpty(propertyName)) return;
            var listView = header.GetAncestor<ListView>();
            Contract.Assert(listView != null);
            SortBase.ApplySort(listView.Items, propertyName, true);
        }
    }
}
