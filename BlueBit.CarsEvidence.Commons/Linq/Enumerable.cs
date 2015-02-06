using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace BlueBit.CarsEvidence.Commons.Linq
{
    public static class _Enumerable
    {
        public static IEnumerable NullAsEmpty_(this IEnumerable @this)
        {
            return @this ?? new object[0];
        }
        public static IEnumerable<T> NullAsEmpty<T>(this IEnumerable<T> @this)
        {
            return @this ?? new T[0];
        }

        public static IEnumerable<T> Castable<T>(this IEnumerable @this)
            where T : class
        {
            foreach (var item in @this)
            {
                var casted = item as T;
                if (casted != null)
                    yield return casted;
            }
        }

        [Obsolete("Use dotNetExt.Each")]
        public static void ForEach<T>(this IEnumerable<T> @this, Action<T> action)
        {
            Contract.Assert(@this != null);
            foreach (var item in @this)
                action(item);
        }

        public static IEnumerable<T> MakeEnumerable<T>(this T @this)
        {
            yield return @this;
        }

        public static T OnlyOneOrDefault<T>(this IEnumerable<T> @this)
        {
            var i = 0;
            var t = default(T);
            foreach (var item in @this)
            {
                if (++i > 1)
                    return default(T);
                t = item;
            }
            return t;
        }
    }
}
