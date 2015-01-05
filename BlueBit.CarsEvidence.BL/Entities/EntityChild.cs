using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace BlueBit.CarsEvidence.BL.Entities
{
    public interface IEntityChild :
        Repositories.IObjectInRepository,
        Alghoritms.IObjectWithGetID
    {
    }

    [DebuggerDisplay("ID={ID}")]
    public abstract class EntityChildBase :
        Repositories.ObjectChildInRepositoryBase,
        IEntityChild
    {
    }
}
