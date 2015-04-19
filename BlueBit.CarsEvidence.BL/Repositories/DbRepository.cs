using BlueBit.CarsEvidence.BL.Entities;
using BlueBit.CarsEvidence.Commons.Diagnostics;
using BlueBit.CarsEvidence.Commons.Templates;
using dotNetExt;
using NHibernate;
using NHibernate.Persister.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;

namespace BlueBit.CarsEvidence.BL.Repositories
{
    public interface IObjectInRepository :
        IObjectWithGetID //TODO-to może być za duże wymaganie
    {
        void Init();
    }

    [DebuggerDisplay("ID={ID}")]
    public abstract class ObjectInRepositoryBase :
        IObjectInRepository,
        IObjectWithSetID
    {
        [Required]
        public virtual long ID { get; set; }

        public abstract void Init();
        public static T Create<T>()
            where T: ObjectInRepositoryBase, new()
        {
            var obj = new T();
            obj.Init();
            return obj;
        }
    }

    [DebuggerDisplay("ID={ID}")]
    public abstract class ObjectChildInRepositoryBase :
        IObjectInRepository,
        IObjectWithSetID
    {
        [Required]
        public virtual long ID { get; set; }

        public abstract void Init();
        public static T Create<T>()
            where T : ObjectChildInRepositoryBase, new()
        {
            var obj = new T();
            obj.Init();
            return obj;
        }
    }

    public abstract class ComponentBase
    {
    }

    public interface IDbRepository
    {
        IList<T> GetAll<T>() where T : class, IObjectInRepository;
        T Get<T>(long id) where T : class, IObjectInRepository;
        T Load<T>(long id) where T : class, IObjectInRepository;

        void Delete<T>(IEnumerable<long> ids) where T : IObjectInRepository;
        void DeleteAll<T>() where T : IObjectInRepository;
        void DeleteAllInDB();

        void Add(IEnumerable<ObjectInRepositoryBase> objs);
        void Update(IEnumerable<ObjectInRepositoryBase> objs);

        IQueryOver<T, T> CreateQuery<T>() where T : class, IObjectInRepository;
    }

    public static class DbRepositoryExtensions
    {
        public static void Delete<T>(this IDbRepository @this, IEnumerable<T> objs)
            where T : EntityBase
        {
            Contract.Assert(@this != null);
            Contract.Assert(objs != null);
            @this.Delete<T>(objs.Select(_ => _.ID));
        }
    }

    public interface IDbRepository<T> :
        IDbRepository
        where T : EntityBase
    {
        bool CanDelete(long id);
    }

    public interface IDbRepositories :
        IDisposable,
        IDbRepository<Address>,
        IDbRepository<Car>,
        IDbRepository<Company>,
        IDbRepository<Period>,
        IDbRepository<Person>,
        IDbRepository<Route>
    {
    }

    public class DbRepositories :
        IDbRepositories
    {
#if DEBUG
        private readonly object _dbgSc = new SingletonChecker<DbRepositories>();
#endif

        private readonly ISessionFactory _sessionFactory;
        private ISession _session = null;
        private ISession Session
        {
            get
            {
                if (_session == null)
                    _session = _sessionFactory.OpenSession();
                return _session;
            }
        }

        public DbRepositories(
            ISessionFactory sessionFactory
            )
        {
            _sessionFactory = sessionFactory;
        }

        public void Dispose()
        {
            if (_session != null)
            {
                _session.Dispose();
                _session = null;
            }
        }

        public void Add(IEnumerable<ObjectInRepositoryBase> objs)
        {
            Contract.Assert(objs != null);
            ExecuteInTransaction(() => objs.Each(_ => Session.Save(_)));
        }

        public void Update(IEnumerable<ObjectInRepositoryBase> objs)
        {
            Contract.Assert(objs != null);
            ExecuteInTransaction(() => objs.Each(_ => Session.Update(_)));
        }

        public void Delete<T>(IEnumerable<long> ids) where T : IObjectInRepository
        {
            Contract.Assert(ids != null);
            ExecuteInTransaction(() => ids.Each(_ => Session.Delete(Session.Load<T>(_))));
        }

        private void DeleteAll_<T>() where T : IObjectInRepository
        {
            var meta = (AbstractEntityPersister)_sessionFactory.GetClassMetadata(typeof(T));
            var sql = string.Format("DELETE FROM \"{0}\"", meta.TableName);
            Session.CreateSQLQuery(sql).ExecuteUpdate();
        }

        public void DeleteAll<T>() where T : IObjectInRepository
        {
            ExecuteInTransaction(() => DeleteAll_<T>());
        }

        public void DeleteAllInDB()
        {
            Dispose();
            ExecuteInTransaction(() =>
            {
                DeleteAll_<PeriodFuelEntry>();
                DeleteAll_<PeriodRouteEntry>();
                DeleteAll_<Period>();
                DeleteAll_<Route>();
                DeleteAll_<Car>();
                DeleteAll_<Person>();
                DeleteAll_<Company>();
                DeleteAll_<Address>();
            });
            Dispose();
        }

        public IList<TEntity> GetAll<TEntity>()
            where TEntity : class, IObjectInRepository
        {
            return Session.QueryOver<TEntity>().List();
        }

        public TEntity Get<TEntity>(long id)
            where TEntity : class, IObjectInRepository
        {
            return Session.Get<TEntity>(id);
        }

        public TEntity Load<TEntity>(long id)
            where TEntity : class, IObjectInRepository
        {
            return Session.Load<TEntity>(id);
        }

        public IQueryOver<T, T> CreateQuery<T>()
            where T : class, IObjectInRepository
        {
            return Session.QueryOver<T>();
        }

        protected void ExecuteInTransaction(Action action)
        {
            using (var transaction = Session.BeginTransaction())
            {
                action();
                transaction.Commit();
            }
        }

        private bool CheckExists<T>(Expression<Func<T, bool>> criteria)
            where T: class, IObjectInRepository
        {
            var result = Session
                .QueryOver<T>()
                .Select(_ => _.ID)
                .Where(criteria)
                .Take(1)
                .SingleOrDefault<long>();
            return result > 0;
        }

        bool IDbRepository<Address>.CanDelete(long id)
        {
            if (CheckExists<Company>(_ => _.Address.ID == id))
                return false;
            if (CheckExists<Route>(_ => _.AddressFrom.ID == id || _.AddressTo.ID == id))
                return false;
            return true;
        }
        bool IDbRepository<Car>.CanDelete(long id)
        {
            if (CheckExists<Period>(_ => _.Car.ID == id))
                return false;
            return false;
        }
        bool IDbRepository<Company>.CanDelete(long id)
        {
            return false;
        }
        bool IDbRepository<Period>.CanDelete(long id)
        {
            return true;
        }
        bool IDbRepository<Person>.CanDelete(long id)
        {
            if (CheckExists<PeriodRouteEntry>(_ => _.Person.ID == id))
                return false;
            return true;
        }
        bool IDbRepository<Route>.CanDelete(long id)
        {
            if (CheckExists<PeriodRouteEntry>(_ => _.Route.ID == id))
                return false;
            return true;
        }
    }
}
