using BlueBit.CarsEvidence.BL.Repositories;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace BlueBit.CarsEvidence.BL.Entities.Components
{
    /// <summary>
    /// Statystyka wartości dla okresu.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IValueStatsBase<T>
        where T : struct
    {
        /// <summary>
        /// Ilość wartości w okresie.
        /// </summary>
        long Count { get; }
        /// <summary>
        /// Suma wartości w okresie.
        /// </summary>
        T ValueSum { get; }
    }

    public class ValueStatsBase<T> :
        ComponentBase,
        IValueStatsBase<T>
        where T : struct
    {
        public long Count { get; set; }
        public T ValueSum { get; set; }
    }

    /// <summary>
    /// Statystyka wartości (narastająco) dla okresu.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IValueStats<T> :
        IValueStatsBase<T>
        where T : struct
    {
        /// <summary>
        /// Suma początkowa (narastająco) wartości w okresie.
        /// Wartość powinna być równa wartości końcowej z okresu poprzedniego o ile takowy występuje.
        /// </summary>
        T ValueBeg { get; }
        /// <summary>
        /// Suma końcowa (narastająco) wartości w okresie.
        /// Wartość powinna być równa wartości początkowej powiększonej o sumę wartości z okresu.
        /// </summary>
        T ValueEnd { get; }
    }

    public class ValueStats<T> :
        ValueStatsBase<T>,
        IValueStats<T>
        where T : struct
    {
        public T ValueBeg { get; set; }
        public T ValueEnd { get; set; }
    }

    public static class ValueStatsExt
    {
        public static ValueStats<T> Clone<T>(this IValueStats<T> @this)
            where T : struct
        {
            Contract.Assert(@this != null);
            return new ValueStats<T>
            {
                Count = @this.Count,
                ValueBeg = @this.ValueBeg,
                ValueEnd = @this.ValueEnd,
                ValueSum = @this.ValueSum,
            };
        }

        public static ValueStats<long> CreateFrom(long beg = 0, long sum = 0, long cnt = 0)
        {
            return new ValueStats<long>() { ValueBeg = beg, ValueEnd = beg + sum, ValueSum = sum, Count = cnt };
        }
        public static ValueStats<long> CreateFrom(IEnumerable<long> items, long beg = 0)
        {
            Contract.Assert(items != null);
            return CreateFrom(beg, items.Sum(), items.Count());
        }

        public static ValueStats<long> Add(this ValueStats<long> @this, IEnumerable<long> items)
        {
            Contract.Assert(@this != null);
            Contract.Assert(items != null);
            var sum = items.Sum();
            @this.Count += items.Count();
            @this.ValueSum += sum;
            @this.ValueEnd = @this.ValueBeg + @this.ValueSum;
            return @this;
        }

/* TODO
        public static ValueStats<decimal> CreateFromBeg(decimal beg)
        {
            return new ValueStats<decimal>() { ValueBeg = beg, ValueEnd = beg };
        }
        public static ValueStats<decimal> CreateFromBegAndSum(decimal beg, decimal sum, long cnt)
        {
            return new ValueStats<decimal>() { ValueBeg = beg, ValueEnd = beg + sum, ValueSum = sum, ValueCnt = cnt };
        }
        public static ValueStats<decimal> CreateFromBegAndSum(decimal beg, IEnumerable<decimal> items)
        {
            var cnt = items.Count();
            var sum = items.Sum();
            return CreateFromBegAndSum(beg, sum, cnt);
        }
 */
    }
}
