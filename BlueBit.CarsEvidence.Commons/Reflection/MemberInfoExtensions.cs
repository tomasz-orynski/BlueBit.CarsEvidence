using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlueBit.CarsEvidence.Commons.Reflection
{
    public static class MemberInfoExtensions
    {
        public static bool HasAttribute<TAttr>(this MemberInfo @this)
            where TAttr : Attribute
        {
            return @this.IsDefined(typeof(TAttr));
        }
        public static MemberInfo OnAttribute<TAttr>(this MemberInfo @this, Action<MemberInfo, TAttr> action)
            where TAttr : Attribute
        {
            Contract.Assert(action != null);
            var attr = @this.GetCustomAttribute<TAttr>();
            if (attr != null)
                action(@this, attr);
            return @this;
        }
        public static bool IsPropertyType<T>(this MemberInfo @this)
        {
            if (@this.MemberType == MemberTypes.Property)
            {
                var pi = (PropertyInfo)@this;
                return pi.PropertyType == typeof(T);
            }
            return false;
        }
        public static MemberInfo OnPropertyType<T>(this MemberInfo @this, Action<MemberInfo> action)
        {
            Contract.Assert(action != null);
            if (@this.MemberType == MemberTypes.Property)
            {
                var pi = (PropertyInfo)@this;
                if (pi.PropertyType == typeof(T))
                    action(@this);
            }
            return @this;
        }
    }
}
