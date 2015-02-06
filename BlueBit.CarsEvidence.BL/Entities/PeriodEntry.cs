using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BlueBit.CarsEvidence.BL.Entities
{
    public class PeriodEntry :
        EntityChildBase
    {
        [Required]
        public virtual Period Period { get; set; }
        [Required]
        public virtual byte Day { get; set; }
        [Required]
        public virtual Person Person { get; set; }
        [Required]
        public virtual Route Route { get; set; }

        public virtual long? Distance { get; set; }

        public override void Init()
        {
        }
    }
}
