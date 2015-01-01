using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BlueBit.CarsEvidence.Commons.Reflection
{
    public static class EventHelper
    {
        public static string GetEventName<T>(Expression<Func<T>> eventExpression)
        {
            var name = (eventExpression.Body as MemberExpression).Member.Name;
            Contract.Assert(!string.IsNullOrWhiteSpace(name));
            return name;
        }
    }
}
