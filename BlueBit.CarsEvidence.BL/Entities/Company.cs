using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BlueBit.CarsEvidence.BL.Entities
{
    public class Company :
        EntityWithCodeBase
    {
        [Required]
        [MaxLength(Configuration.Consts.LengthText)]
        public virtual string Name { get; set; }
        [MaxLength(Configuration.Consts.LengthIdentifierNIP)]
        public virtual string IdentifierNIP { get; set; }
        [MaxLength(Configuration.Consts.LengthIdentifierREGON)]
        public virtual string IdentifierREGON { get; set; }
        [MaxLength(Configuration.Consts.LengthInfo)]
        public virtual string Info { get; set; }

        public virtual Address Address { get; set; }

        public override void Init()
        {
        }
    }
}
