using BlueBit.CarsEvidence.Commons.Templates;
using FluentNHibernate.Automapping;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BlueBit.CarsEvidence.BL.Entities
{
    public interface IEntity :
        Repositories.IObjectInRepository,
        IObjectWithGetID
    {
        IEnumerable<IEntity> GetDependentEntities();
    }

    [DebuggerDisplay("ID={ID}")]
    public abstract class EntityBase :
        Repositories.ObjectInRepositoryBase,
        IEntity
    {
        public virtual IEnumerable<IEntity> GetDependentEntities()
        {
            return Enumerable.Empty<IEntity>();
        }
    }

    public interface IEntityCfg
    {
        void Configure(AutoPersistenceModel model);
    }

    public abstract class EntityCfgBase<T> :
        IEntityCfg
        where T : IEntity
    {
        public void Configure(AutoPersistenceModel model)
        {
            model.Override<T>(ConfigureMapping);
        }

        protected abstract void ConfigureMapping(AutoMapping<T> map);
    }
}
