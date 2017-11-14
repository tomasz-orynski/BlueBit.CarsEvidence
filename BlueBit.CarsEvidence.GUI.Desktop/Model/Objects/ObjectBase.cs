using BlueBit.CarsEvidence.BL.Alghoritms;
using BlueBit.CarsEvidence.BL.Entities;
using BlueBit.CarsEvidence.Commons.Reflection;
using BlueBit.CarsEvidence.Commons.Templates;
using dotNetExt;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;

namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Objects
{
    public enum EntityType : byte
    {
        Address,
        Company,
        Car,
        Person,
        Period,
        Route,
    }

    public static class EntityTypeDict
    {
        public static readonly IReadOnlyDictionary<string, EntityType> Name2Value = Enum
            .GetValues(typeof(EntityType))
            .Cast<EntityType>()
            .ToDictionary(_ => _.ToString());

        public static EntityType GetValueForEntityType(Type t)
        {
            var name = t.Name;
            return Name2Value[name];
        }
        public static EntityType GetValueForEntityType<T>()
            where T : EntityBase
        {
            var name = typeof(T).Name;
            return Name2Value[name];
        }
        public static EntityType GetValueForObjectType<T>()
            where T : ObjectWithIDBase
        {
            var name = typeof(T).Name;
            return Name2Value[name];
        }
    }

    public interface IObjectForEntityType :
        IObjectForType<EntityType>
    {
    }

    public interface IObjectWithDescription
    {
        string Description { get; }
    }
    public interface IObjectWithDescriptionForToolTip
    {
        string DescriptionForToolTip { get; }
    }
    public static class ObjectWithDescriptionsExtensions
    {
        public static string GetDescription<T>(this T @this)
            where T : IObjectWithGetID, IObjectWithGetCode
        {
            Contract.Assert(@this != null);

            var sb = new StringBuilder();
#if DEBUG
            sb.AppendFormat("GD: »{0}« #{1}", @this.Code, @this.ID);
#else
            sb.Append(@this.Code);
#endif
            return sb.ToString();
        }
        public static string GetDescriptionForToolTip<T>(this T @this)
            where T : IObjectWithGetID, IObjectWithGetCode, IObjectWithGetInfo
        {
            Contract.Assert(@this != null);

            var sb = new StringBuilder();
#if DEBUG
            sb.AppendFormat("GDTT: »{0}« #{1}", @this.Code, @this.ID);
#else
            sb.Append(@this.Code);
#endif
            var info = @this.Info;
            if (!string.IsNullOrWhiteSpace(info))
            {
                sb.AppendLine();
                sb.AppendLine("---");
                sb.Append(info);
            }

            return sb.ToString();
        }
    }

    public interface IObject
    {
    }

    public abstract class ObjectBase :
        ObservableObject,
        IObject,
        IObjectForEntityType
    {
        public Type ForEntityType
        {
            get
            {
                var type = GetType()
                    .GetAttribute<Attributes.EntityTypeAttribute>()
                    .EntityType;
                return type;
            }
        }
        public EntityType ForType { get { return EntityTypeDict.GetValueForEntityType(ForEntityType); } }

        protected void _Set<T>(
            ref T value, T newValue,
            Action onChange = null,
            [CallerMemberName] string propertyName = null
            )
        {
            Contract.Assert(!string.IsNullOrWhiteSpace(propertyName));
            if (Set<T>(propertyName, ref value, newValue))
            {
                if (onChange != null) onChange();
                OnSet(propertyName);
                GetPropertyDependency(propertyName).Each(RaisePropertyChanged);
            }
        }
        protected void _SetColl<T>(
            ref ObservableCollection<T> value, ObservableCollection<T> newValue,
            Action onChange = null,
            [CallerMemberName] string propertyName = null
            )
        {
            Contract.Assert(!string.IsNullOrWhiteSpace(propertyName));
            if (Set<ObservableCollection<T>>(propertyName, ref value, newValue))
            {
                if (onChange != null)
                {
                    //TODO: odwiązać poprzednią kolekcję (zdarzenia).
                    value.CollectionChanged += (_, __) => onChange();
                    onChange();
                }
                OnSet(propertyName);
                GetPropertyDependency(propertyName).Each(RaisePropertyChanged);
            }
        }
        protected virtual void OnSet(string propertyName) { }

        private readonly static Dictionary<Type, Dictionary<string, HashSet<string>>> _propertyDependencies = new Dictionary<Type, Dictionary<string, HashSet<string>>>();

        public interface IRegistrator<TObj>
        {
            IRegistrator<TObj> Add<TDP>(Expression<Func<TObj, TDP>> dependencyProperty, params Expression<Func<TObj, object>>[] basedOnProperties);
        }
        private class Registrator<TObj> :
            IRegistrator<TObj>
        {
            public Dictionary<string, HashSet<string>> typePropertyDependencies;

            public IRegistrator<TObj> Add<TDP>(Expression<Func<TObj, TDP>> dependencyProperty, params Expression<Func<TObj, object>>[] basedOnProperties)
            {
                var dependencyPropertyName = PropertyHelper<TObj>.GetPropertyName<TDP>(dependencyProperty);
                basedOnProperties.Each(property =>
                {
                    var propertyName = PropertyHelper.GetPropertyName_(property);
                    HashSet<string> propertyDependencies = null;
                    if (!typePropertyDependencies.TryGetValue(propertyName, out propertyDependencies))
                    {
                        propertyDependencies = new HashSet<string>();
                        typePropertyDependencies.Add(propertyName, propertyDependencies);
                    }
                    propertyDependencies.Add(dependencyPropertyName);
                });
                return this;
            }
        }

        protected static IRegistrator<T> RegisterPropertyDependency<T>()
        {
            var type = typeof(T);

            Dictionary<string, HashSet<string>> typePropertyDependencies = null;
            if (!_propertyDependencies.TryGetValue(type, out typePropertyDependencies))
            {
                typePropertyDependencies = new Dictionary<string, HashSet<string>>();
                _propertyDependencies.Add(type, typePropertyDependencies);
            }
            return new Registrator<T>() { typePropertyDependencies = typePropertyDependencies };
        }

        private IEnumerable<string> GetPropertyDependency(string propertyName)
        {
            for (var type = GetType(); type != null; type = type.BaseType)
            {
                Dictionary<string, HashSet<string>> typePropertyDependencies = null;
                if (_propertyDependencies.TryGetValue(type, out typePropertyDependencies))
                {
                    HashSet<string> propertyDependencies = null;
                    if (typePropertyDependencies.TryGetValue(propertyName, out propertyDependencies))
                        return propertyDependencies;
                }
            }
            return Enumerable.Empty<string>();
        }

#if DEBUG
        public override string ToString()
        {
            return string.Format("DBG:Type={0}", GetType().Name);
        }
#endif
    }

    [DebuggerDisplay("ID={ID}")]
    public abstract class ObjectWithIDBase :
        ObjectBase,
        IObjectWithGetID,
        IObjectWithSetID,
        IEquatable<ObjectWithIDBase>
    {
        /// <summary>
        /// ID nie powinno podlegać zmianie, stąd brak notyfikacji o zmianach.
        /// </summary>Type ForType { get; }
        public long ID { get; set; }

        public bool Equals(ObjectWithIDBase other)
        {
            if (other == null) return false;
            if (!GetType().Equals(other.GetType())) return false;
            return ID == other.ID;
        }
        public override bool Equals(object obj) { return Equals(obj as ObjectWithIDBase); }
        public override int GetHashCode() { return ID.GetHashCode(); }

#if DEBUG
        public override string ToString()
        {
            return string.Format("DBG:Type={0},ID={1}", GetType().Name, ID);
        }
#endif
    }

    public static class ObjectWithIDBaseExtensions
    {
        public static bool IsFromDb(this ObjectWithIDBase @this)
        {
            Contract.Assert(@this != null);
            return @this.ID > 0;
        }
    }
}
