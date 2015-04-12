using BlueBit.CarsEvidence.Commons.Templates;
using System.Diagnostics;

namespace BlueBit.CarsEvidence.BL.Entities
{
    public interface IEntityChild :
        Repositories.IObjectInRepository,
        IObjectWithGetID
    {
    }

    [DebuggerDisplay("ID={ID}")]
    public abstract class EntityChildBase :
        Repositories.ObjectChildInRepositoryBase,
        IEntityChild
    {
    }
}
