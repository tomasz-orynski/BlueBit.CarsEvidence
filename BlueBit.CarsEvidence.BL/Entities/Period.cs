using FluentNHibernate.Automapping;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueBit.CarsEvidence.BL.Entities
{
    [DataContract(Namespace = Consts.NamespaceEntities, IsReference = true)]
    public class Period : 
        EntityBase
    {
        [Required]
        [DataMember]
        public virtual int Year { get; set; }
        [Required]
        [DataMember]
        public virtual byte Month { get; set; }
        [Required]
        [DataMember]
        public virtual Car Car { get; set; }

        [NotMapped]
        [DataMember(Name = "PeriodEntries")]
        public virtual PeriodEntry[] _PeriodEntries { 
            get { return PeriodEntries == null ? null : PeriodEntries.ToArray(); } 
            set { PeriodEntries = new HashSet<PeriodEntry>(value); } 
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
