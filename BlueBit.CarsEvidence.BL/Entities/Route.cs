using BlueBit.CarsEvidence.Commons.Templates;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;

namespace BlueBit.CarsEvidence.BL.Entities
{
    public class Route :
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
        public virtual Address AddressFrom { get; set; }
        [Required]
        public virtual Address AddressTo { get; set; }
        [Required]
        public virtual long Distance { get; set; }
        [Required]
        public virtual bool DistanceIsInBothDirections { get; set; }

        public virtual ISet<PeriodRouteEntry> PeriodRouteEntries { get; set; }

        public override void Init()
        {
            PeriodRouteEntries = PeriodRouteEntries ?? new HashSet<PeriodRouteEntry>();
        }

        public override IEnumerable<IEntity> GetDependentEntities()
        {
            return PeriodRouteEntries.Select(_ => _.Period);
        }
    }

    public static class RouteExtensions
    {
        public static PeriodRouteEntry AddPeriodRouteEntry(this Route @this, PeriodRouteEntry entry)
        {
            Contract.Assert(@this != null);
            Contract.Assert(entry != null);
            Contract.Assert(entry.Route == null);

            entry.Route = @this;
            @this.PeriodRouteEntries.Add(entry);
            return entry;
        }
    }
}
