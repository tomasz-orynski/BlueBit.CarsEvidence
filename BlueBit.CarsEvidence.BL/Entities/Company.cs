using System.Runtime.Serialization;

namespace BlueBit.CarsEvidence.BL.Entities
{
    [DataContract(Namespace = Consts.NamespaceEntities, IsReference = true)]
    public class Company :
        EntityWithCodeBase
    {
        public override void Init()
        {
        }
    }
}
