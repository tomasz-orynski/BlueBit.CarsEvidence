using FluentNHibernate.Automapping;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace BlueBit.CarsEvidence.BL.Entities
{
    public interface IEntity :
        Repositories.IObjectInRepository,
        Alghoritms.IObjectWithGetID
    {
        IEnumerable<IEntity> GetDependentEntities();
    }

    public interface IEntityWithCode :
        IEntity,
        Alghoritms.IObjectWithGetCode
    {
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

    [DebuggerDisplay("ID={ID},Code={Code}")]
    public abstract class EntityWithCodeBase :
        EntityBase,
        IEntityWithCode
    {
        [Required]
        [MaxLength(Configuration.Consts.LengthCode)]
        public virtual string Code { get; set; }
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
