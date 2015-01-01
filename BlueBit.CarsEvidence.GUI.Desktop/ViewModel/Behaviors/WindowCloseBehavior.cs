using BlueBit.CarsEvidence.Commons.Reflection;
using System.Windows;
using System.Windows.Interactivity;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Behaviors
{
    public class WindowCloseBehavior : Behavior<Window>
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(
            PropertyHelper<WindowCloseBehavior>.GetPropertyName(_ => _.ViewModel),
            typeof(IViewModelWithClose),
            typeof(WindowCloseBehavior),
            new FrameworkPropertyMetadata(null, OnViewModelChanged));

        public object ViewModel
        {
            get { return GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var @this = (WindowCloseBehavior)d;
            var @viewModelOld = (IViewModelWithClose)e.OldValue;
            var @viewModelNew = (IViewModelWithClose)e.NewValue;

            if (@viewModelOld != null)
                @viewModelOld.Closed -= @this.OnViewModelClosed;
            if (@viewModelNew != null)
                @viewModelNew.Closed += @this.OnViewModelClosed;
        }

        private void OnViewModelClosed(object sender, System.EventArgs e)
        {
            var window = (Window)AssociatedObject;
            window.Close();
        }
    }
}
