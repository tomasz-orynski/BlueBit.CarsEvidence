using BlueBit.CarsEvidence.Commons.Templates;
using System.ComponentModel.DataAnnotations;

namespace BlueBit.CarsEvidence.BL.Entities
{
    public class PeriodRouteEntry :
        EntityChildBase,
        IObjectWithGetInfo
    {
        [MaxLength(Configuration.Consts.LengthInfo)]
        public virtual string Info { get; set; }

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
