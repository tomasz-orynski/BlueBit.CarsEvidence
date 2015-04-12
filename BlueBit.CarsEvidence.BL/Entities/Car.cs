using BlueBit.CarsEvidence.BL.Entities.Components;
using BlueBit.CarsEvidence.BL.Entities.Enums;
using BlueBit.CarsEvidence.Commons.Templates;
using FluentNHibernate.Automapping;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace BlueBit.CarsEvidence.BL.Entities
{
    public class Car :
        EntityBase,
        IObjectWithGetCode,
        IObjectWithGetInfo
    {
        [Required]
        [MaxLength(Configuration.Consts.LengthCode)]
        public virtual string Code { get; set; }
        [MaxLength(Configuration.Consts.LengthInfo)]
        public virtual string Info { get; set; }

        [Required]
        [MaxLength(Configuration.Consts.LengthRegisterNumber)]
        public virtual string RegisterNumber { get; set; }
        [MaxLength(Configuration.Consts.LengthText)]
        public virtual string BrandInfo { get; set; }
        public virtual ValueState<long> EvidenceBeg { get; set; }
        public virtual ValueState<long> EvidenceEnd { get; set; }
        [Required]
        public virtual FuelType FuelType { get; set; }

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
