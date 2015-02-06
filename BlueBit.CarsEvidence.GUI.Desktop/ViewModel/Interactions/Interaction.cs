using System.Windows;
using System.Windows.Media;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Interactions
{
    public static class InteractionBase
    {
        public static T GetAncestor<T>(this DependencyObject obj)
            where T : DependencyObject
        {
            for (;;)
            {
                obj = VisualTreeHelper.GetParent(obj);
                if (obj == null) return null;

                var t = obj as T;
                if (t != null) return t;
            }
        }
    }
}
