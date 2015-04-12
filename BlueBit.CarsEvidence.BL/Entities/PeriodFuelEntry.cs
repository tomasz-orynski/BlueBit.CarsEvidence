using BlueBit.CarsEvidence.BL.Entities.Components;
using BlueBit.CarsEvidence.BL.Entities.Enums;
using BlueBit.CarsEvidence.Commons.Templates;
using System.ComponentModel.DataAnnotations;

namespace BlueBit.CarsEvidence.BL.Entities
{
    public class PeriodFuelEntry :
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
        public virtual TimeOfDay TimeOfDay { get; set; }
        [Required]
        public virtual Person Person { get; set; }
        [Required]
        public virtual FuelPurchase Purchase { get; set; }

        public override void Init()
        {
        }
    }
}
