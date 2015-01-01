using FluentNHibernate.Automapping;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BlueBit.CarsEvidence.BL.Entities
{
    [DataContract(Namespace = Consts.NamespaceEntities, IsReference = false)]
    public class PeriodEntry :
        EntityChildBase
    {
        [Required]
        public virtual Period Period { get; set; }
        [Required]
        [DataMember]
        public virtual byte Day { get; set; }
        [Required]
        [DataMember]
        public virtual Person Person { get; set; }
        [Required]
        [DataMember]
        public virtual Route Route { get; set; }
        [Required]
        [DataMember]
        public virtual long Distance { get; set; }

        public override void Init()
        {
        }
    }
}
