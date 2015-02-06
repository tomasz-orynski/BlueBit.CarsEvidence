using dotNetExt;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;
using System.Windows.Media.Imaging;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Interactions.Behaviors
{
    public class IconBehavior :
        Behavior<Window>
    {
        public static readonly DependencyProperty IconPathProperty = BehaviorBase
            .ForType<IconBehavior>.RegisterProperty(_ => _.IconPath);

        public string IconPath
        {
            get { return (string)GetValue(IconPathProperty); }
            set { SetValue(IconPathProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            var path = IconPath ?? "Resources/Icons/globe.ico";
            path = string.Format("pack://application:,,,/{0}", path);
            var icoUri = new Uri(path, UriKind.RelativeOrAbsolute);
            AssociatedObject.Icon = BitmapFrame.Create(icoUri);
        }
    }
}
