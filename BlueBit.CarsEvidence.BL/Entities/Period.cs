using BlueBit.CarsEvidence.BL.Entities.Components;
using BlueBit.CarsEvidence.Commons.Templates;
using FluentNHibernate.Automapping;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace BlueBit.CarsEvidence.BL.Entities
{
    public class Period : 
        EntityBase,
        IObjectWithGetInfo
    {
        [MaxLength(Configuration.Consts.LengthInfo)]
        public virtual string Info { get; set; }

        [Required]
        public virtual int Year { get; set; }
        [Required]
        public virtual byte Month { get; set; }
        [Required]
        public virtual Car Car { get; set; }

        [Required]
        public virtual ValueStats<long> RouteStats { get; set; }
        [Required]
        public virtual PurchaseStats FuelStats { get; set; }

        public virtual ISet<PeriodRouteEntry> RouteEntries { get; set; }
        public virtual ISet<PeriodFuelEntry> FuelEntries { get; set; }

        public override void Init()
        {
            RouteEntries = RouteEntries ?? new HashSet<PeriodRouteEntry>();
            FuelEntries = FuelEntries ?? new HashSet<PeriodFuelEntry>();
        }
    }

    public static class PeriodExtensions
    {
        public static PeriodRouteEntry AddRouteEntry(this Period @this, PeriodRouteEntry entry)
        {
            Contract.Assert(@this != null);
            Contract.Assert(entry != null);
            Contract.Assert(entry.Period == null);

            entry.Period = @this;
            @this.RouteEntries.Add(entry);
            return entry;
        }

        public static PeriodFuelEntry AddFuelEntry(this Period @this, PeriodFuelEntry entry)
        {
            Contract.Assert(@this != null);
            Contract.Assert(entry != null);
            Contract.Assert(entry.Period == null);

            entry.Period = @this;
            @this.FuelEntries.Add(entry);
            return entry;
        }
    }

    public class PeriodCfg :
        EntityCfgBase<Period>
    {
        protected override void ConfigureMapping(AutoMapping<Period> map)
        {
            map
                .HasMany(_ => _.RouteEntries)
                .Cascade.All();
            map
                .HasMany(_ => _.FuelEntries)
                .Cascade.All();
        }
    }
}
