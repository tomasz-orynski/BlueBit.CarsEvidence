using System.Windows;
using System.Windows.Interactivity;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Interactions.Behaviors
{
    public class FocusBehavior :
        Behavior<UIElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Focus();
        }  
    }
}
