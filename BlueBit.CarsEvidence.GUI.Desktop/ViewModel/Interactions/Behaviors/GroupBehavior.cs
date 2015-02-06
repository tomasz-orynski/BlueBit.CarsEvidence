using dotNetExt;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Interactions.Behaviors
{
    public static class GroupBase
    {
        public static void ApplyGroup(ICollectionView view, string propertyNames)
        {
            if (string.IsNullOrEmpty(propertyNames)) return;
            var pn = propertyNames.Split(',');

            view.GroupDescriptions.Clear();
            pn.Each(_ => view.GroupDescriptions.Add(new PropertyGroupDescription(_)));
        }
    }

    public class GroupDefaultBehavior :
        Behavior<ItemsControl>
    {
        public static readonly DependencyProperty PropertyNamesProperty = BehaviorBase
            .ForType<GroupDefaultBehavior>.RegisterProperty(_ => _.PropertyNames);

        public string PropertyNames
        {
            get { return (string)GetValue(PropertyNamesProperty); }
            set { SetValue(PropertyNamesProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            GroupBase.ApplyGroup(AssociatedObject.Items, PropertyNames);
        }
    }
}
