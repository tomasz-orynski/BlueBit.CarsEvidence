using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlueBit.CarsEvidence.Commons.Helpers;
using System.Diagnostics.Contracts;

namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.View.General.Helpers
{
    public class EV<T> :
        IViewGeneralObject,
        IEquatable<EV<T>>,
        IEquatable<T>
        where T: struct, IConvertible //enum
    {
        private readonly T _value;

        public T Value { get { return _value; } }
        public string Name { get { return _value.ToString(); } } //TODO
        public string Description { get { return Name; } }
        public string DescriptionForToolTip { get { return Name; } }

        public EV(T value)
        {
            _value = value;
        }

        public int CompareTo(object obj) { return this.CompareDescriptionTo(obj); }
        public override int GetHashCode() { return _value.GetHashCode(); }
        public override bool Equals(object other)
        {
            if (other == null) return false;
            return Equals(other as EV<T>);
        }
        public bool Equals(EV<T> other) 
        {
            if (other == null) return false;
            return _value.Equals(other.Value); 
        }
        public bool Equals(T other) 
        { 
            return _value.Equals(other); 
        }
        public static bool operator==(EV<T> @this, T other) 
        {
            Contract.Assert(@this != null);
            return @this.Equals(other); 
        }
        public static bool operator !=(EV<T> @this, T other)
        {
            Contract.Assert(@this != null);
            return !@this.Equals(other);
        }
    }

    public static class EVExtensions<T>
        where T: struct, IConvertible //enum
    {
        private static readonly Lazy<ObservableCollection<EV<T>>> _items = ValueHelper.InitLazy(() => {
            var values = Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(_ => new EV<T>(_));
            return new ObservableCollection<EV<T>>(values);
        });

        public static ObservableCollection<EV<T>> Items { get { return _items.Value; } }
        public static EV<T> GetItem(T value) { return _items.Value.Single(_ => _ == value); }
    }
}
