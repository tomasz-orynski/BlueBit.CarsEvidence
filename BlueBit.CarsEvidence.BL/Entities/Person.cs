using BlueBit.CarsEvidence.Commons.Templates;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace BlueBit.CarsEvidence.BL.Entities
{
    public class Person :
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
        [MaxLength(Configuration.Consts.LengthText)]
        public virtual string FirstName { get; set; }
        [Required]
        [MaxLength(Configuration.Consts.LengthText)]
        public virtual string LastName { get; set; }

        public virtual ISet<PeriodRouteEntry> PeriodRouteEntries { get; set; }
        public virtual ISet<PeriodFuelEntry> PeriodFuelEntries { get; set; }

        public override void Init()
        {
            PeriodRouteEntries = PeriodRouteEntries ?? new HashSet<PeriodRouteEntry>();
            PeriodFuelEntries = PeriodFuelEntries ?? new HashSet<PeriodFuelEntry>();
        }
    }

    public static class PersonExtensions
    {
        public static PeriodRouteEntry AddPeriodRouteEntry(this Person @this, PeriodRouteEntry entry)
        {
            Contract.Assert(@this != null);
            Contract.Assert(entry != null);
            Contract.Assert(entry.Person == null);

            entry.Person = @this;
            @this.PeriodRouteEntries.Add(entry);
            return entry;
        }
        public static PeriodFuelEntry AddPeriodFuelEntry(this Person @this, PeriodFuelEntry entry)
        {
            Contract.Assert(@this != null);
            Contract.Assert(entry != null);
            Contract.Assert(entry.Person == null);

            entry.Person = @this;
            @this.PeriodFuelEntries.Add(entry);
            return entry;
        }
    }
}
