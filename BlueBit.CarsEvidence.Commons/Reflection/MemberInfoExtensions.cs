using System;
using System.Collections.Generic;
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
            return Attribute.GetCustomAttribute(@this, typeof(TAttr)) != null;
        }

        public static TAttr GetAttribute<TAttr>(this MemberInfo @this)
            where TAttr : Attribute
        {
            return (TAttr)Attribute.GetCustomAttribute(@this, typeof(TAttr));
        }
    }
}
