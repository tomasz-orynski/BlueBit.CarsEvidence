using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueBit.CarsEvidence.Commons.Reflection
{
    public static class TypeExtensions
    {
        public static IEnumerable<Type> GetImplementingTypes(this Type type, IEnumerable<Type> types)
        {
            return types.GetImplementingTypes(type);
        }
        public static IEnumerable<Type> GetImplementingTypes(this IEnumerable<Type> types, Type type)
        {
            Contract.Assert(types != null);
            Contract.Assert(type != null);
            Contract.Assert(type.IsInterface);
            return types.Where(t => t.GetInterfaces().Contains(type) && !t.IsAbstract && !t.IsGenericType);
        }
        public static IEnumerable<Type> GetDerivedTypes(this IEnumerable<Type> types, Type type)
        {
            Contract.Assert(types != null);
            Contract.Assert(type != null);
            Contract.Assert(type.IsClass);
            return types.Where(t => t.IsSubclassOf(type) && !t.IsAbstract && !t.IsGenericType);
        }
        public static IEnumerable<Type> GetDerivedTypes<T>(this IEnumerable<Type> types)
        {
            return types.GetDerivedTypes(typeof(T));
        }

        public static bool HasAttribute<TAttr>(this Type @this)
            where TAttr : Attribute
        {
            return Attribute.GetCustomAttribute(@this, typeof(TAttr)) != null;
        }

        public static TAttr GetAttribute<TAttr>(this Type @this)
            where TAttr : Attribute
        {
            return (TAttr)Attribute.GetCustomAttribute(@this, typeof(TAttr));
        }

        private static TAttr GetAttribute<T, TAttr>()
            where TAttr : Attribute
        {
            return GetAttribute<TAttr>(typeof(T));
        }
    }
}
