using BlueBit.CarsEvidence.Commons.Linq;
using BlueBit.CarsEvidence.BL.Entities;
using BlueBit.CarsEvidence.BL.Repositories;
using BlueBit.CarsEvidence.Commons.Diagnostics;
using BlueBit.CarsEvidence.Commons.Templates;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects;
using dotNetExt;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;

namespace BlueBit.CarsEvidence.GUI.Desktop.Model
{
    public interface IGeneralObjects<T> :
        IObjectForEntityType
        where T : Objects.ObjectBase
    {
        T Get(long id);
    }

    public interface IViewObjects<T> :
        IGeneralObjects<T>
        where T : Objects.View.ViewObjectBase
    {
        ObservableCollection<T> Items { get; }
    }

    public interface IEditObjects<T> :
        IGeneralObjects<T>
        where T : Objects.Edit.EditObjectBase
    {
        T Create();
        void Save(T obj);

        bool CanDelete(long id);
        void Delete(long id);
    }

    public static class EditObjectsExtensions
    {
        public static bool CanDelete<T>(this IEditObjects<T> @this, Objects.ObjectBase obj)
            where T : Objects.Edit.EditObjectBase
        {
            Contract.Assert(@this != null);
            Contract.Assert(obj != null);
            Contract.Assert(@this.ForType == obj.ForType);
            return @this.CanDelete(obj.ID);
        }
        public static void Delete<T>(this IEditObjects<T> @this, Objects.ObjectBase obj)
            where T : Objects.Edit.EditObjectBase
        {
            Contract.Assert(@this != null);
            Contract.Assert(obj != null);
            Contract.Assert(@this.ForType == obj.ForType);
            @this.Delete(obj.ID);
        }
    }

    public interface IRepositoryGeneral
    {
        void Import(IEnumerable<EntityBase> entites);
    }

    public interface IRepositoryViewObjects<T>
        where T : Objects.View.ViewObjectBase
    {
        IViewObjects<T> GetViewObjects();
    }

    public interface IRepositoryEditObjects<T>
        where T : Objects.Edit.EditObjectBase
    {
        IEditObjects<T> GetEditObjects();
    }

    partial class Repositories
    {
        private interface  _IViewObjectsCache
        {
            void Clear();
            void HandleAddOnImport(EntityBase entity);
        }

        private interface _IViewObjectsCache<TEntity> :
            _IViewObjectsCache
            where TEntity : EntityBase
        {
            void HandleAdd(TEntity entity);
            void HandleUpdate(TEntity entity);
            void HandleRemove(long id);
        }

        private abstract class _ObjectsBase<TEntity> :
            IObjectForEntityType
            where TEntity : EntityBase
        {
            private readonly IDbRepository<TEntity> _entitiesRepository;
            protected IDbRepository<TEntity> EntitiesRepository { get { return _entitiesRepository; } }

            private readonly Func<IEnumerable<_IViewObjectsCache<TEntity>>> _getCaches;
            protected IEnumerable<_IViewObjectsCache<TEntity>> Caches { get { return _getCaches(); } }

            public Type ForEntityType { get { return typeof(TEntity); } }
            public EntityType ForType { get { return EntityTypeDict.GetValueForEntityType<TEntity>(); } }

            protected _ObjectsBase(
                IDbRepository<TEntity> entitiesRepository,
                Func<IEnumerable<_IViewObjectsCache<TEntity>>> getCaches)
            {
                _entitiesRepository = entitiesRepository;
                _getCaches = getCaches;
            }

            public virtual void Delete(long id)
            {
                _entitiesRepository.Delete<TEntity>(id.MakeEnumerable());
                Caches.Each(c => c.HandleRemove(id));
            }
            public bool CanDelete(long id)
            {
                return _entitiesRepository.CanDelete(id);
            }
        }

        private abstract class _ObjectsBase<T, TEntity> :
            _ObjectsBase<TEntity>,
            IGeneralObjects<T>
            where TEntity : EntityBase
            where T : Objects.ObjectBase
        {
#if DEBUG
            private readonly object _dbgSc = new SingletonChecker<_ObjectsBase<T, TEntity>>();
#endif
            protected readonly Lazy<IConverterFromEntity<T, TEntity>> _convertFromEntity;

            protected _ObjectsBase(
                IDbRepository<TEntity> entitiesRepository,
                Func<IEnumerable<_IViewObjectsCache<TEntity>>> getCaches
                )
                : base(entitiesRepository, getCaches)
            {
                _convertFromEntity = new Lazy<IConverterFromEntity<T, TEntity>>(Configuration.Settings.ResolveType<IConverterFromEntity<T, TEntity>>);
            }

            public virtual T Get(long id)
            {
                return _convertFromEntity.Value.Create(EntitiesRepository.Get<TEntity>(id));
            }
        }

        private class _ViewObjects<T, TEntity> :
            _ObjectsBase<T, TEntity>,
            _IViewObjectsCache<TEntity>,
            IViewObjects<T>
            where T : Objects.View.ViewObjectBase
            where TEntity : EntityBase
        {
            protected readonly Lazy<Tuple<ObservableCollection<T>, Dictionary<long, T>>> _items;
            public ObservableCollection<T> Items { get { return _items.Value.Item1; } }

            public _ViewObjects(
                IDbRepository<TEntity> entitiesRepository,
                Func<IEnumerable<_IViewObjectsCache<TEntity>>> getCaches
                )
                : base(entitiesRepository, getCaches)
            {
                _items = new Lazy<Tuple<ObservableCollection<T>, Dictionary<long, T>>>(GetItems);
            }
            private Tuple<ObservableCollection<T>, Dictionary<long, T>> GetItems()
            {
                var coll = new ObservableCollection<T>(EntitiesRepository.GetAll<TEntity>().Select(_convertFromEntity.Value.Create));
                return Tuple.Create(coll, coll.ToDictionary(_ => _.ID));
            }

            public override T Get(long id)
            {
                return _items.Value.Item2[id];
            }

            public void Clear()
            {
                if (_items.IsValueCreated)
                {
                    _items.Value.Item1.Clear();
                    _items.Value.Item2.Clear();
                }
            }

            public void HandleAddOnImport(EntityBase entity)
            {
                Contract.Assert(entity is TEntity);
                HandleAdd((TEntity)entity);
            }

            public void HandleAdd(TEntity entity)
            {
                if (_items.IsValueCreated)
                {
                    var obj = _convertFromEntity.Value.Create(entity);
                    _items.Value.Item2[obj.ID] = obj;
                    _items.Value.Item1.Add(obj);
                }
            }
            public void HandleUpdate(TEntity entity)
            {
                if (_items.IsValueCreated)
                {
                    var obj = _items.Value.Item2[entity.ID];
                    _convertFromEntity.Value.Update(entity, obj);
                }
            }
            public void HandleRemove(long id)
            {
                if (_items.IsValueCreated)
                {
                    var col = _items.Value.Item1;
                    var idx = col.Count;
                    while (--idx >= 0)
                    {
                        if (col[idx].ID == id)
                            col.RemoveAt(idx);
                    }
                    _items.Value.Item2.Remove(id);
                }
            }
        }

        private class _EditObjects<T, TEntity> :
            _ObjectsBase<T, TEntity>,
            IEditObjects<T>
            where T : Objects.Edit.EditObjectBase
            where TEntity : EntityBase, new()
        {
            protected readonly Lazy<IConverterToEntity<T, TEntity>> _convertToEntity;

            public _EditObjects(
                IDbRepository<TEntity> entitiesRepository,
                Func<IEnumerable<_IViewObjectsCache<TEntity>>> getCaches
                )
                : base(entitiesRepository, getCaches)
            {
                _convertToEntity = new Lazy<IConverterToEntity<T, TEntity>>(Configuration.Settings.ResolveType<IConverterToEntity<T, TEntity>>);
            }

            public T Create()
            {
                var entity = new TEntity();
                return _convertFromEntity.Value.Create(entity);
            }

            public void Save(T obj)
            {
                if (obj.IsFromDb())
                {
                    var entity = _convertToEntity.Value.Update(obj, EntitiesRepository.Get<TEntity>(obj.ID));
                    EntitiesRepository.Update(entity.MakeEnumerable());
                    Caches.Each(c => c.HandleUpdate(entity));
                }
                else
                {
                    var entity = _convertToEntity.Value.Create(obj);
                    EntitiesRepository.Add(entity.MakeEnumerable());
                    Caches.Each(c => c.HandleAdd(entity));
                }
            }
        }
    }
}
