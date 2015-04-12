using System;
using System.Diagnostics.Contracts;

namespace BlueBit.CarsEvidence.Commons.Helpers
{
    public static class ValueExtensions
    {
        public static TValue GetSafeValue<T, TValue>(this T @this, Func<T, TValue> getter)
            where T: class
        {
            Contract.Assert(getter != null);
            return @this == null 
                ? default(TValue)
                : getter(@this);
        }
        public static TValue GetSafeNullableValue<T, TValue>(this T? @this, Func<T, TValue> getter)
            where T : struct
        {
            Contract.Assert(getter != null);
            return @this == null
                ? default(TValue)
                : getter(@this.Value);
        }
    }


    public static class ValueHelper
    {
        public static Lazy<TValue> InitLazy<TValue>(this Func<TValue> getter)
        {
            Contract.Assert(getter != null);
            return new Lazy<TValue>(getter);
        }
    }
}
