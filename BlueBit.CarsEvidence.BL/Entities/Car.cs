using FluentNHibernate.Automapping;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace BlueBit.CarsEvidence.BL.Entities
{
    public class Car :
        EntityWithCodeBase
    {
        [Required]
        [MaxLength(Configuration.Consts.LengthRegisterNumber)]
        public virtual string RegisterNumber { get; set; }

        [MaxLength(Configuration.Consts.LengthText)]
        public virtual string BrandInfo { get; set; }

        public virtual DateTime EvidenceDateBegin { get; set; }

        public virtual DateTime? EvidenceDateEnd { get; set; }

        public virtual long EvidenceCounterBegin { get; set; }

        public virtual long? EvidenceCounterEnd { get; set; }

        public virtual ISet<Period> Periods { get; set; }

        public override void Init()
        {
            Periods = Periods ?? new HashSet<Period>();
        }
    }

    public static class CarExtensions
    {
        public static Period AddPeriod(this Car @this, Period period)
        {
            Contract.Assert(@this != null);
            Contract.Assert(period != null);
            Contract.Assert(period.Car == null);

            period.Car = @this;
            @this.Periods.Add(period);
            return period;
        }
    }

    public class CarCfg :
        EntityCfgBase<Car>
    {
        protected override void ConfigureMapping(AutoMapping<Car> map)
        {
            map.HasMany(_ => _.Periods)
                .Cascade.All();
        }
    }
}
