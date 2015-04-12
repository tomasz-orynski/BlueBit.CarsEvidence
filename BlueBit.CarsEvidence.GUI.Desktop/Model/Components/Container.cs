using GalaSoft.MvvmLight;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Components
{
    public abstract class ContainerBase :
        ObservableObject
    {
        protected void _Set<T>(
            ref T value, T newValue,
            Action onChange = null,
            [CallerMemberName] string propertyName = null
            )
        {
            Contract.Assert(!string.IsNullOrWhiteSpace(propertyName));
            if (Set<T>(propertyName, ref value, newValue))
            {
                if (onChange != null)
                    onChange();
            }
        }
    }

    public abstract class ItemContainerBase<TValue> :
        ContainerBase
        where TValue : new()
    {
        private TValue item = new TValue();
        public TValue Item { get { return item; } set { _Set(ref item, value); } }
    }

    public abstract class ItemsContainerBase<TValue> :
        ContainerBase
    {
        private ObservableCollection<TValue> items;
        public ObservableCollection<TValue> Items { get { return items; } set { _Set(ref items, value); } }
    }
}
