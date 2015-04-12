using BlueBit.CarsEvidence.BL.Entities.Attributes;
using BlueBit.CarsEvidence.BL.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace BlueBit.CarsEvidence.BL.Entities.Components
{
    /// <summary>
    /// Patrz opis <see cref="IValueStatsBase<T>"/>.
    /// </summary>
    public interface IPurchaseStatsBase
    {
        long Count { get; }
        [PrecisionScale(Configuration.Consts.VolumeSPrecision, Configuration.Consts.VolumeScale)]
        decimal VolumeSum { get; }
        [PrecisionScale(Configuration.Consts.AmountSPrecision, Configuration.Consts.AmountScale)]
        decimal AmountSum { get; }
    }

    public class PurchaseStatsBase :
        ComponentBase,
        IPurchaseStatsBase
    {
        public long Count { get; set; }
        [PrecisionScale(Configuration.Consts.VolumeSPrecision, Configuration.Consts.VolumeScale)]
        public decimal VolumeSum { get; set; }
        [PrecisionScale(Configuration.Consts.AmountSPrecision, Configuration.Consts.AmountScale)]
        public decimal AmountSum { get; set; }
    }

    /// <summary>
    /// Patrz opis <see cref="IValueStats<T>"/>
    /// </summary>
    public interface IPurchaseStats :
        IPurchaseStatsBase
    {
        decimal VolumeBeg { get; }
        decimal VolumeEnd { get; }
        decimal AmountBeg { get; }
        decimal AmountEnd { get; }
    }

    public class PurchaseStats :
        PurchaseStatsBase,
        IPurchaseStats
    {
        [PrecisionScale(Configuration.Consts.VolumeSPrecision, Configuration.Consts.VolumeScale)]
        public decimal VolumeBeg { get; set; }
        [PrecisionScale(Configuration.Consts.VolumeSPrecision, Configuration.Consts.VolumeScale)]
        public decimal VolumeEnd { get; set; }
        [PrecisionScale(Configuration.Consts.AmountSPrecision, Configuration.Consts.AmountScale)]
        public decimal AmountBeg { get; set; }
        [PrecisionScale(Configuration.Consts.AmountSPrecision, Configuration.Consts.AmountScale)]
        public decimal AmountEnd { get; set; }
    }

    public static class PurchaseStatsExt
    {
        public static PurchaseStats Clone(this IPurchaseStats @this)
        {
            Contract.Assert(@this != null);
            return new PurchaseStats
            {
                Count = @this.Count,
                VolumeSum = @this.VolumeSum,
                VolumeBeg = @this.VolumeBeg,
                VolumeEnd = @this.VolumeBeg + @this.VolumeSum,
                AmountSum = @this.AmountSum,
                AmountBeg = @this.AmountBeg,
                AmountEnd = @this.AmountBeg + @this.AmountSum,
            };
        }

        public static PurchaseStats CreateFrom<T>(
            IEnumerable<T> items, Func<T, decimal> getVolume, Func<T, decimal> getAmount, 
            decimal volumeBeg = 0, 
            decimal amountBeg = 0)
        {
            Contract.Assert(items != null);
            Contract.Assert(getVolume != null);
            Contract.Assert(getAmount != null);

            var volume = items.Sum(getVolume);
            var amount = items.Sum(getAmount);
            return new PurchaseStats() 
            { 
                Count = items.Count(),
                VolumeSum = volume,
                VolumeBeg = volumeBeg, 
                VolumeEnd = volumeBeg + volume,
                AmountSum = amount,
                AmountBeg = amountBeg, 
                AmountEnd = amountBeg + amount,
            };
        }

        public static PurchaseStats Add<T>(this PurchaseStats @this, IEnumerable<T> items, Func<T, decimal> getVolume, Func<T, decimal> getAmount)
        {
            Contract.Assert(@this != null);
            Contract.Assert(items != null);
            Contract.Assert(getVolume != null);
            Contract.Assert(getAmount != null);

            var volume = items.Sum(getVolume);
            var amount = items.Sum(getAmount);
            @this.Count += items.Count();
            @this.VolumeSum += volume;
            @this.VolumeEnd = @this.VolumeBeg + @this.VolumeSum;
            @this.AmountSum += amount;
            @this.AmountEnd += @this.AmountBeg + @this.AmountSum;
            return @this;
        }

    }
}
