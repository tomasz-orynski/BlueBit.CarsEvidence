using BlueBit.CarsEvidence.BL.Alghoritms;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;
using System.Linq;

namespace BlueBit.CarsEvidence.BL.Entities
{
    public class Route :
        EntityWithCodeBase
    {
        [Required]
        public virtual Address AddressFrom { get; set; }
        [Required]
        public virtual Address AddressTo { get; set; }
        [Required]
        public virtual long Distance { get; set; }
        [Required]
        public virtual bool DistanceIsInBothDirections { get; set; }
        [MaxLength(Configuration.Consts.LengthInfo)]
        public virtual string Info { get; set; }

        public virtual ISet<PeriodEntry> PeriodEntries { get; set; }

        public override void Init()
        {
            PeriodEntries = PeriodEntries ?? new HashSet<PeriodEntry>();
        }

        public override IEnumerable<IEntity> GetDependentEntities()
        {
            return PeriodEntries.Select(_ => _.Period);
        }
    }

    public static class RouteExtensions
    {
        public static PeriodEntry AddPeriodEntry(this Route @this, PeriodEntry entry)
        {
            Contract.Assert(@this != null);
            Contract.Assert(entry != null);
            Contract.Assert(entry.Route == null);

            entry.Route = @this;
            @this.PeriodEntries.Add(entry);
            return entry;
        }
    }
}
