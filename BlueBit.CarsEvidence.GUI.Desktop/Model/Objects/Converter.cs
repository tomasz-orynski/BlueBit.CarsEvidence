using BlueBit.CarsEvidence.BL.Entities;
using BlueBit.CarsEvidence.BL.Repositories;
using BlueBit.CarsEvidence.Commons.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using dotNetExt;

namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Objects
{
    public interface IConverterInitiator
    {
        void Initialize();
    }

    public interface IConverterInstance
    {
        object Instance { get; }
    }

    public class ConverterInstance<T> :
        IConverterInstance
        where T: IConverterInitiator
    {
#if DEBUG
        private readonly object _dbgSc = new SingletonChecker<ConverterInstance<T>>();
#endif
        private readonly T _instance;
        public object Instance { get { return _instance; } }

        public ConverterInstance(T instance)
        {
            instance.Initialize();
            _instance = instance;
        }
    }

    public interface IConverterFromEntity<TObject, in TEntity>
        where TObject : ObjectWithIDBase
        where TEntity : EntityBase
    {
        TObject Create(TEntity src);
        TObject Update(TEntity src, TObject dst);
    }

    public interface IConverterToEntity<in TObject, TEntity>
        where TObject : ObjectWithIDBase
        where TEntity : EntityBase
    {
        TEntity Create(TObject src);
        TEntity Update(TObject src, TEntity dst);
    }

    public interface IConverterFromEntityChild<in TParentContext, TObject, in TEntity>
        where TObject : ObjectWithIDBase
        where TEntity : IObjectInRepository
    {
        TObject Create(TParentContext ctx, TEntity src);
        TObject Update(TParentContext ctx, TEntity src, TObject dst);
    }

    public interface IConverterToEntityChild<in TParentContext, in TObject, TEntity>
        where TObject : ObjectWithIDBase
        where TEntity : IObjectInRepository
    {
        TEntity Create(TParentContext ctx, TObject src);
        TEntity Update(TParentContext ctx, TObject src, TEntity dst);
    }

    public static class ConverterExtensions
    {
        public static ObservableCollection<TObj> Convert<TParentObj, TParentEntity, TObj, TEntity>(
            this IConverterFromEntityChild<Tuple<TParentObj, TParentEntity>, TObj, TEntity> converter,
            TParentObj parentObject,
            TParentEntity parentEntity,
            IEnumerable<TEntity> entities
            )
            where TParentObj : ObjectWithIDBase
            where TObj : ObjectWithIDBase
            where TParentEntity : IEntity
            where TEntity : IEntityChild
        {
            Contract.Assert(converter != null);

            entities = entities ?? Enumerable.Empty<TEntity>();
            return new ObservableCollection<TObj>(
                entities
                    .Select(_ => converter.Create(Tuple.Create(parentObject, parentEntity), _)));
        }

        public static ISet<TEntity> Convert<TParentObj, TObj, TParentEntity, TEntity>(
            this IConverterToEntityChild<Tuple<TParentObj, TParentEntity>, TObj, TEntity> converter,
            TParentObj parentObject,
            TParentEntity parentEntity,
            IEnumerable<TObj> objects,
            ISet<TEntity> entities
            )
            where TParentObj : ObjectWithIDBase
            where TObj : ObjectWithIDBase
            where TParentEntity : IEntity
            where TEntity : IEntityChild
        {
            Contract.Assert(converter != null);

            objects = objects ?? Enumerable.Empty<TObj>();
            var ctx = Tuple.Create(parentObject, parentEntity);
            if (entities == null)
            {
                return new HashSet<TEntity>(
                    objects
                        .Select(_ => converter.Create(ctx, _)));
            }

            var entitiesMap = entities
                .ToDictionary(_ => _.ID);
            var entitiesNew = objects
                .Select(_ => _.ID > 0 
                    ? converter.Update(ctx, _, entitiesMap[_.ID])
                    : converter.Create(ctx, _)
                );
            return new HashSet<TEntity>(entitiesNew);
        }


        public static TObj ConvertByGet<TObj>(this Func<IGeneralObjects<TObj>> objects, long id)
            where TObj : ObjectWithIDBase
        {
            Contract.Assert(objects != null);
            return objects().Get(id);
        }

        public static TObj ConvertByGet<TObj, TEntity>(this Func<IGeneralObjects<TObj>> objects, TEntity entity)
            where TObj : ObjectWithIDBase
            where TEntity : BL.Entities.EntityBase
        {
            if (entity == null) return default(TObj);
            return ConvertByGet<TObj>(objects, entity.ID);
        }

        public static TEntity ConvertByLoad<TEntity>(this Func<IDbRepositories> repository, long id)
            where TEntity : BL.Entities.EntityBase
        {
            Contract.Assert(repository != null);
            return repository().Load<TEntity>(id);
        }
    }
}
