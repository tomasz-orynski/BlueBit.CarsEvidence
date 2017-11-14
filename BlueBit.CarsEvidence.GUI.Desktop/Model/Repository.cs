using BlueBit.CarsEvidence.BL.Entities;
using BlueBit.CarsEvidence.BL.Repositories;
using BlueBit.CarsEvidence.Commons.Diagnostics;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using dotNetExt;

namespace BlueBit.CarsEvidence.GUI.Desktop.Model
{
    public partial class Repositories :
        IRepositoryGeneral,
        IRepositoryViewObjects<Objects.View.General.Address>,
        IRepositoryViewObjects<Objects.View.General.Car>,
        IRepositoryViewObjects<Objects.View.General.Company>,
        IRepositoryViewObjects<Objects.View.General.Person>,
        IRepositoryViewObjects<Objects.View.General.Route>,
        IRepositoryViewObjects<Objects.View.Panels.Address>,
        IRepositoryViewObjects<Objects.View.Panels.Car>,
        IRepositoryViewObjects<Objects.View.Panels.Person>,
        IRepositoryViewObjects<Objects.View.Panels.Period>,
        IRepositoryViewObjects<Objects.View.Panels.Route>,
        IRepositoryEditObjects<Objects.Edit.Documents.Address>,
        IRepositoryEditObjects<Objects.Edit.Documents.Car>,
        IRepositoryEditObjects<Objects.Edit.Documents.Company>,
        IRepositoryEditObjects<Objects.Edit.Documents.Period>,
        IRepositoryEditObjects<Objects.Edit.Documents.Person>,
        IRepositoryEditObjects<Objects.Edit.Documents.Route>
    {
#if DEBUG
        private readonly object _dbgSc = new SingletonChecker<Repositories>();
#endif
        private readonly IDbRepositories _entitiesRepository;

        public Repositories(
            IDbRepositories entitiesRepository
            )
        {
            _entitiesRepository = entitiesRepository;
        }

        public void Dispose()
        {
            _entitiesRepository.Dispose();
        }

        public void Import(IEnumerable<EntityBase> entites)
        {
            _entitiesRepository.DeleteAllInDB();

            _viewObjects.Values
                .SelectMany(_ => _.Values)
                .Cast<_IViewObjectsCache>()
                .Each(_ => _.Clear());

            _entitiesRepository.Add(entites);

            //TODO-pogrupować po typie, ale i posortować po typie (niezależność od innych typów).
            entites.Each(entity =>
            {
                IDictionary<Type, object> vo;
                if (_viewObjects.TryGetValue(entity.GetType(), out vo))
                    vo.Values
                        .Cast<_IViewObjectsCache>()
                        .Each(_ => _.HandleAddOnImport(entity));

            });
        }


        private static TColl GetObjectsInternal<TColl, T, TEntity>(
            IDictionary<Type, IDictionary<Type, object>> objectsColl,
            Func<TColl> creator
            )
            where TColl : IGeneralObjects<T>
            where T: Objects.ObjectWithIDBase
            where TEntity: EntityBase
        {
            IDictionary<Type, object> objectsDict;
            object objects;

            if (!objectsColl.TryGetValue(typeof(TEntity), out objectsDict))
            {
                objectsDict = new Dictionary<Type, object>();
                objectsColl.Add(typeof(TEntity), objectsDict);
            }

            if (!objectsDict.TryGetValue(typeof(T), out objects))
            {
                objects = creator();
                objectsDict.Add(typeof(T), objects);
            }
            return (TColl)objects;
        }

        //[TEntity,T]=>ViewObjects<T>
        private readonly IDictionary<Type, IDictionary<Type, object>> _viewObjects = new Dictionary<Type, IDictionary<Type, object>>();

        private IViewObjects<T> GetViewObjectsInternal<T, TEntity>(IDbRepository<TEntity> entitiesRepository)
            where T : Objects.View.ViewObjectBase
            where TEntity : EntityBase
        {
            return GetObjectsInternal<IViewObjects<T>, T, TEntity>(
                _viewObjects,
                () => new _ViewObjects<T, TEntity>(entitiesRepository, GetViewObjectsCaches<TEntity>)
                );
        }

        //[TEntity,T]=>EditObjects<T>
        private readonly IDictionary<Type, IDictionary<Type, object>> _editObjects = new Dictionary<Type, IDictionary<Type, object>>();

        private IEditObjects<T> GetEditObjectsInternal<T, TEntity>(IDbRepository<TEntity> entitiesRepository)
            where T : Objects.Edit.EditObjectBase
            where TEntity : EntityBase, new()
        {
            return GetObjectsInternal<IEditObjects<T>, T, TEntity>(
                _editObjects,
                () => new _EditObjects<T, TEntity>(entitiesRepository, GetViewObjectsCaches<TEntity>)
                );
        }

        private IEnumerable<_IViewObjectsCache<TEntity>> GetViewObjectsCaches<TEntity>()
            where TEntity : EntityBase
        {
            IDictionary<Type, object> viewObjectsDict;
            if (!_viewObjects.TryGetValue(typeof(TEntity), out viewObjectsDict))
                return Enumerable.Empty<_IViewObjectsCache<TEntity>>();

            return viewObjectsDict.Values.Cast<_IViewObjectsCache<TEntity>>();
        }


        IViewObjects<Objects.View.General.Address> IRepositoryViewObjects<Objects.View.General.Address>.GetViewObjects()
        {
            return GetViewObjectsInternal<Objects.View.General.Address, BL.Entities.Address>(_entitiesRepository);
        }
        IViewObjects<Objects.View.General.Car> IRepositoryViewObjects<Objects.View.General.Car>.GetViewObjects()
        {
            return GetViewObjectsInternal<Objects.View.General.Car, BL.Entities.Car>(_entitiesRepository);
        }
        IViewObjects<Objects.View.General.Company> IRepositoryViewObjects<Objects.View.General.Company>.GetViewObjects()
        {
            return GetViewObjectsInternal<Objects.View.General.Company, BL.Entities.Company>(_entitiesRepository);
        }
        IViewObjects<Objects.View.General.Person> IRepositoryViewObjects<Objects.View.General.Person>.GetViewObjects()
        {
            return GetViewObjectsInternal<Objects.View.General.Person, BL.Entities.Person>(_entitiesRepository);
        }
        IViewObjects<Objects.View.General.Route> IRepositoryViewObjects<Objects.View.General.Route>.GetViewObjects()
        {
            return GetViewObjectsInternal<Objects.View.General.Route, BL.Entities.Route>(_entitiesRepository);
        }

        IViewObjects<Objects.View.Panels.Address> IRepositoryViewObjects<Objects.View.Panels.Address>.GetViewObjects()
        {
            return GetViewObjectsInternal<Objects.View.Panels.Address, BL.Entities.Address>(_entitiesRepository);
        }
        IViewObjects<Objects.View.Panels.Car> IRepositoryViewObjects<Objects.View.Panels.Car>.GetViewObjects()
        {
            return GetViewObjectsInternal<Objects.View.Panels.Car, BL.Entities.Car>(_entitiesRepository);
        }
        IViewObjects<Objects.View.Panels.Person> IRepositoryViewObjects<Objects.View.Panels.Person>.GetViewObjects()
        {
            return GetViewObjectsInternal<Objects.View.Panels.Person, BL.Entities.Person>(_entitiesRepository);
        }
        IViewObjects<Objects.View.Panels.Period> IRepositoryViewObjects<Objects.View.Panels.Period>.GetViewObjects()
        {
            return GetViewObjectsInternal<Objects.View.Panels.Period, BL.Entities.Period>(_entitiesRepository);
        }
        IViewObjects<Objects.View.Panels.Route> IRepositoryViewObjects<Objects.View.Panels.Route>.GetViewObjects()
        {
            return GetViewObjectsInternal<Objects.View.Panels.Route, BL.Entities.Route>(_entitiesRepository);
        }

        IEditObjects<Objects.Edit.Documents.Address> IRepositoryEditObjects<Objects.Edit.Documents.Address>.GetEditObjects()
        {
            return GetEditObjectsInternal<Objects.Edit.Documents.Address, BL.Entities.Address>(_entitiesRepository);
        }
        IEditObjects<Objects.Edit.Documents.Car> IRepositoryEditObjects<Objects.Edit.Documents.Car>.GetEditObjects()
        {
            return GetEditObjectsInternal<Objects.Edit.Documents.Car, BL.Entities.Car>(_entitiesRepository);
        }
        IEditObjects<Objects.Edit.Documents.Company> IRepositoryEditObjects<Objects.Edit.Documents.Company>.GetEditObjects()
        {
            return GetEditObjectsInternal<Objects.Edit.Documents.Company, BL.Entities.Company>(_entitiesRepository);
        }
        IEditObjects<Objects.Edit.Documents.Person> IRepositoryEditObjects<Objects.Edit.Documents.Person>.GetEditObjects()
        {
            return GetEditObjectsInternal<Objects.Edit.Documents.Person, BL.Entities.Person>(_entitiesRepository);
        }
        IEditObjects<Objects.Edit.Documents.Period> IRepositoryEditObjects<Objects.Edit.Documents.Period>.GetEditObjects()
        {
            return GetEditObjectsInternal<Objects.Edit.Documents.Period, BL.Entities.Period>(_entitiesRepository);
        }
        IEditObjects<Objects.Edit.Documents.Route> IRepositoryEditObjects<Objects.Edit.Documents.Route>.GetEditObjects()
        {
            return GetEditObjectsInternal<Objects.Edit.Documents.Route, BL.Entities.Route>(_entitiesRepository);
        }
    }
}
