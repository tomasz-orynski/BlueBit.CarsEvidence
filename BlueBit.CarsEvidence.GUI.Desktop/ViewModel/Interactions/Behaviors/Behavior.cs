using BlueBit.CarsEvidence.Commons.Linq;
using BlueBit.CarsEvidence.Commons.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Interactivity;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Interactions.Behaviors
{
    public static class BehaviorBase
    {
        public static class ForType<T>
            where T : Behavior
        {
            public static DependencyProperty RegisterProperty<TVal>(
                Expression<Func<T, TVal>> expression
                )
            {
                return DependencyProperty.Register(
                    PropertyHelper<T>.GetPropertyName<TVal>(expression),
                    typeof(TVal),
                    typeof(T));
            }
            public static DependencyProperty RegisterProperty<TVal>(
                Expression<Func<T, TVal>> expression,
                PropertyMetadata typeMetadata
                )
            {
                return DependencyProperty.Register(
                    PropertyHelper<T>.GetPropertyName<TVal>(expression),
                    typeof(TVal),
                    typeof(T),
                    typeMetadata);
            }
        }

        public static IEnumerable<T> GetBehaviors<T>(this DependencyObject obj)
            where T: Behavior
        {
            var bs = Interaction.GetBehaviors(obj);
            if (bs == null) return null;
            return bs.Castable<T>();
        }

        public static T GetBehavior<T>(this DependencyObject obj)
            where T : Behavior
        {
            var bs = Interaction.GetBehaviors(obj);
            if (bs == null) return null;
            return bs.Castable<T>().FirstOrDefault();
        }

    }
}
