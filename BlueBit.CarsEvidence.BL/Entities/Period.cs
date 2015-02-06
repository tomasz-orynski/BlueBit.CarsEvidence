using BlueBit.CarsEvidence.BL.Alghoritms;
using BlueBit.CarsEvidence.Commons.Linq;
using FluentNHibernate.Automapping;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;

namespace BlueBit.CarsEvidence.BL.Entities
{
    public class Period : 
        EntityBase
    {
        [Required]
        public virtual int Year { get; set; }
        [Required]
        public virtual byte Month { get; set; }
        [Required]
        public virtual Car Car { get; set; }
        [Required]
        public virtual long DistanceTotal {
            get { return PeriodEntries.GetDistanceTotal(); }
            set { }
        }

        public virtual ISet<PeriodEntry> PeriodEntries { get; set; }

        public override void Init()
        {
            PeriodEntries = PeriodEntries ?? new HashSet<PeriodEntry>();
        }
    }

    public static class PeriodExtensions
    {
        public static PeriodEntry AddPeriodEntry(this Period @this, PeriodEntry entry)
        {
            Contract.Assert(@this != null);
            Contract.Assert(entry != null);
            Contract.Assert(entry.Period == null);

            entry.Period = @this;
            @this.PeriodEntries.Add(entry);
            return entry;
        }
    }

    public class PeriodCfg :
        EntityCfgBase<Period>
    {
        protected override void ConfigureMapping(AutoMapping<Period> map)
        {
            map.HasMany(_ => _.PeriodEntries)
                .Cascade.All();
        }
    }
}
