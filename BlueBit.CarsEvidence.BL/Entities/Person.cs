using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace BlueBit.CarsEvidence.BL.Entities
{
    public class Person :
        EntityWithCodeBase
    {
        [Required]
        public virtual string FirstName { get; set; }
        [Required]
        public virtual string LastName { get; set; }

        public virtual ISet<PeriodEntry> PeriodEntries { get; set; }

        public override void Init()
        {
            PeriodEntries = PeriodEntries ?? new HashSet<PeriodEntry>();
        }
    }

    public static class PersonExtensions
    {
        public static PeriodEntry AddPeriodEntry(this Person @this, PeriodEntry entry)
        {
            Contract.Assert(@this != null);
            Contract.Assert(entry != null);
            Contract.Assert(entry.Person == null);

            entry.Person = @this;
            @this.PeriodEntries.Add(entry);
            return entry;
        }
    }
}
