using BlueBit.CarsEvidence.Commons.Templates;
using System.ComponentModel.DataAnnotations;

namespace BlueBit.CarsEvidence.BL.Entities
{
    public class Company :
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
        public virtual string Name { get; set; }
        [MaxLength(Configuration.Consts.LengthIdentifierNIP)]
        public virtual string IdentifierNIP { get; set; }
        [MaxLength(Configuration.Consts.LengthIdentifierREGON)]
        public virtual string IdentifierREGON { get; set; }

        public virtual Address Address { get; set; }

        public override void Init()
        {
        }
    }
}
