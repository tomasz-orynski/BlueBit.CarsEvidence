using BlueBit.CarsEvidence.BL.Alghoritms;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace BlueBit.CarsEvidence.BL.Entities
{
    [DataContract(Namespace = Consts.NamespaceEntities, IsReference = true)]
    public class Route :
        EntityWithCodeBase,
        IRoute
    {
        [Required]
        [DataMember]
        public virtual Address AddressFrom { get; set; }
        [Required]
        [DataMember]
        public virtual Address AddressTo { get; set; }
        [Required]
        [DataMember]
        public virtual long Distance { get; set; }
        [Required]
        [DataMember]
        public virtual bool DistanceIsInBothDirections { get; set; }

        public virtual ISet<PeriodEntry> PeriodEntries { get; set; }

        public override void Init()
        {
            PeriodEntries = PeriodEntries ?? new HashSet<PeriodEntry>();
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
            if (entry.Distance == default(long))
                entry.Distance = @this.Distance;
            return entry;
        }
    }
}
