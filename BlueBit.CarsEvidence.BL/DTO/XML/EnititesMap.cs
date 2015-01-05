using AutoMapper;
using Entities = BlueBit.CarsEvidence.BL.Entities;
using BlueBit.CarsEvidence.BL.Repositories;
using BlueBit.CarsEvidence.Commons.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Diagnostics.Contracts;

namespace BlueBit.CarsEvidence.BL.DTO.XML
{
    [DataContract(Namespace = Consts.Namespace_DTO_XML)]
    public abstract class DTOBase 
    {
        [DataMember(Order = 0)]
        public long ID { get; set; }
    }

    [DataContract(Namespace = Consts.Namespace_DTO_XML)]
    public abstract class DTOWithCodeBase :
        DTOBase
    {
        [DataMember(Order = 1)]
        public string Code { get; set; }
    }

    [DataContract(Namespace = Consts.Namespace_DTO_XML)]
    public class Address :
        DTOWithCodeBase
    {
        [DataMember]
        public string PostalCode { get; set; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string Street { get; set; }
        [DataMember]
        public string BuildingNo { get; set; }
        [DataMember]
        public string LocalNo { get; set; }
    }

    [DataContract(Namespace = Consts.Namespace_DTO_XML)]
    public class Company :
        DTOWithCodeBase
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string IdentifierNIP { get; set; }
        [DataMember]
        public string IdentifierREGON { get; set; }
        [DataMember]
        public long AddressID { get; set; }
    }

    [DataContract(Namespace = Consts.Namespace_DTO_XML)]
    public class Car :
        DTOWithCodeBase
    {
        [DataMember]
        public string RegisterNumber { get; set; }
        [DataMember]
        public string BrandInfo { get; set; }
        [DataMember]
        public DateTime EvidenceDateBegin { get; set; }
        [DataMember]
        public DateTime? EvidenceDateEnd { get; set; }
        [DataMember]
        public long EvidenceCounterBegin { get; set; }
        [DataMember]
        public long? EvidenceCounterEnd { get; set; }
    }

    [DataContract(Namespace = Consts.Namespace_DTO_XML)]
    public class Person :
        DTOWithCodeBase
    {
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
    }

    [DataContract(Namespace = Consts.Namespace_DTO_XML)]
    public class Period :
        DTOBase
    {
        [DataMember(Order = 10)]
        public int Year { get; set; }
        [DataMember(Order = 11)]
        public byte Month { get; set; }
        [DataMember(Order = 12)]
        public long CarID { get; set; }
        [DataMember(Order = 20)]
        public PeriodEntry[] PeriodEntries { get; set; }
    }

    [DataContract(Namespace = Consts.Namespace_DTO_XML)]
    public class PeriodEntry :
        DTOBase
    {
        [DataMember]
        public byte Day { get; set; }
        [DataMember]
        public long PersonID { get; set; }
        [DataMember]
        public long RouteID { get; set; }
        [DataMember]
        public long Distance { get; set; }
    }

    [DataContract(Namespace = Consts.Namespace_DTO_XML)]
    public class Route :
        DTOWithCodeBase
    {
        [DataMember]
        public long AddressFromID { get; set; }
        [DataMember]
        public long AddressToID { get; set; }
        [DataMember]
        public long Distance { get; set; }
        [DataMember]
        public bool DistanceIsInBothDirections { get; set; }
    }

    [DataContract(Namespace = Consts.Namespace_DTO_XML)]
    public class DataHeader
    {
        [DataMember(Order = 0)]
        public string AppName { get; set; }
        [DataMember(Order = 1)]
        public string AppVersion { get; set; }
        [DataMember(Order = 2, Name = "CreateTime")]
        public DateTime DateTime { get; set; }
    }

    [DataContract(Namespace = Consts.Namespace_DTO_XML)]
    public abstract class DataBase
    {
    }

    [DataContract(Namespace = Consts.Namespace_DTO_XML, IsReference = false, Name = "Root")]
    public class DataRoot<TData>
        where TData : DataBase
    {
        [DataMember(Order = 0, Name = "Header")]
        public DataHeader Header { get; set; }

        [DataMember(Order = 1, Name = "Data")]
        public TData Data { get; set; }
    }

    [DataContract(Namespace = Consts.Namespace_DTO_XML)]
    public class DataEXP :
        DataBase
    {
        private readonly IDbRepositories _entitiesRepository;

        static DataEXP()
        {
            Mapper.CreateMap<Entities.Address, Address>();
            Mapper.CreateMap<Entities.Car, Car>();
            Mapper.CreateMap<Entities.Company, Company>();
            Mapper.CreateMap<Entities.Person, Person>();
            Mapper.CreateMap<Entities.Period, Period>();
            Mapper.CreateMap<Entities.PeriodEntry, PeriodEntry>();
            Mapper.CreateMap<Entities.Route, Route>();
        }

        public DataEXP(
            IDbRepositories entitiesRepository
        )
        {
            _entitiesRepository = entitiesRepository;
        }

        private IEnumerable<TDTO> GetAll<TEntity, TDTO>()
             where TEntity : class, IObjectInRepository
        {
            return _entitiesRepository
                .GetAll<TEntity>()
                .Select(Mapper.Map<TEntity, TDTO>);
        }

        [DataMember(Order = 0)]
        public Company Company
        {
            get
            {
                return GetAll<Entities.Company, Company>().Single();
            }
            set { }
        }

        [DataMember(Order = 1)]
        public Person[] Persons { get { return GetAll<Entities.Person, Person>().ToArray(); } }
        [DataMember(Order = 2)]
        public Car[] Cars { get { return GetAll<Entities.Car, Car>().ToArray(); } }
        [DataMember(Order = 3)]
        public Address[] Addresses { get { return GetAll<Entities.Address, Address>().ToArray(); } }
        [DataMember(Order = 4)]
        public Route[] Routes { get { return GetAll<Entities.Route, Route>().ToArray(); } }
        [DataMember(Order = 5)]
        public Period[] Periods { get { return GetAll<Entities.Period, Period>().ToArray(); } }
    }

    [DataContract(Namespace = Consts.Namespace_DTO_XML)]
    public class DataIMP : DataBase
    {
        private List<Action> _actions = new List<Action>();
        private Entities.Company _company;
        private IDictionary<long, Entities.Address> _addresses;
        private IDictionary<long, Entities.Person> _persons;
        private IDictionary<long, Entities.Car> _cars;
        private IDictionary<long, Entities.Route> _routes;
        private IDictionary<long, Entities.Period> _periods;

        static DataIMP()
        {
            PrepareMap<Address, Entities.Address>();
            PrepareMap<Car, Entities.Car>();
            PrepareMap<Company, Entities.Company>();
            PrepareMap<Person, Entities.Person>();
            PrepareMap<Period, Entities.Period>()
                .ForMember(
                    dst => dst.PeriodEntries,
                    cfg => cfg.Ignore());
            PrepareMap<PeriodEntry, Entities.PeriodEntry>();
            PrepareMap<Route, Entities.Route>();
        }

        static IMappingExpression<TSrc, TDst> PrepareMap<TSrc, TDst>()
            where TSrc: DTOBase
            where TDst: class, IObjectInRepository
        {
            return Mapper.CreateMap<TSrc, TDst>()
                .BeforeMap(
                    (src, dst) => { dst.Init(); }
                )
                .ForMember(
                    dst => dst.ID,
                    cfg => cfg.Ignore())
                .ForSourceMember(
                    src => src.ID,
                    cfg => cfg.Ignore());
        }

        private void AddActions(params Action[] actions)
        {
            if (_actions == null)
                _actions = new List<Action>();

            _actions.AddRange(actions);
        }

        private void AddActionForDict<TDst>(
            Func<IDictionary<long, TDst>> getDict, 
            long srcID, 
            Action<TDst> action)
        {
            if (_actions == null)
                _actions = new List<Action>();

            var dict = getDict();
            if (dict != null)
                action(dict[srcID]);
            else
                _actions.Add(() => action(getDict()[srcID]));
        }


        [DataMember(Order = 0)]
        public Company Company {
            get { return null; }
            set 
            {
                _company = Mapper.Map<Company, Entities.Company>(value);
                AddActionForDict(
                    () => _addresses, value.AddressID,
                    (i) => Entities.AddressExtensions.AddCompany(i, _company));
            }
        }
        [DataMember(Order = 1)]
        public Person[] Persons
        {
            get { return null; }
            set
            {
                _persons = value
                    .ToDictionary(_ => _.ID, Mapper.Map<Person, Entities.Person>);
            }
        }

        [DataMember(Order = 2)]
        public Car[] Cars
        {
            get { return null; }
            set
            {
                _cars = value
                    .ToDictionary(_ => _.ID, Mapper.Map<Car, Entities.Car>);
            }
        }

        [DataMember(Order = 3)]
        public Address[] Addresses
        {
            get { return null; }
            set
            {
                _addresses = value
                    .ToDictionary(_ => _.ID, Mapper.Map<Address, Entities.Address>);
            }
        }
        
        [DataMember(Order = 4)]
        public Route[] Routes
        {
            get { return null; }
            set
            {
                _routes = value
                    .ToDictionary(_ => _.ID, _ => {
                        var obj = Mapper.Map<Route, Entities.Route>(_);
                        AddActionForDict(
                            () => _addresses, _.AddressFromID, 
                            (i) => Entities.AddressExtensions.AddRouteFrom(i, obj));
                        AddActionForDict(
                            () => _addresses, _.AddressToID, 
                            (i) => Entities.AddressExtensions.AddRouteTo(i, obj));
                        return obj;
                    });
            }
        }
        
        [DataMember(Order = 5)]
        public Period[] Periods
        {
            get { return null; }
            set
            {
                _periods = value
                    .ToDictionary(_ => _.ID, _ =>
                    {
                        var obj = Mapper.Map<Period, Entities.Period>(_);
                        _.PeriodEntries.Each(__ =>
                            {
                                var objChild = Mapper.Map<PeriodEntry, Entities.PeriodEntry>(__);
                                Entities.PeriodExtensions.AddPeriodEntry(obj, objChild);
                                AddActionForDict(
                                    () => _persons, __.PersonID,
                                    (i) => Entities.PersonExtensions.AddPeriodEntry(i, objChild));
                                AddActionForDict(
                                    () => _routes, __.RouteID,
                                    (i) => Entities.RouteExtensions.AddPeriodEntry(i, objChild));
                            });
                        AddActions(
                            () => Entities.CarExtensions.AddPeriod(_cars[_.CarID], obj)
                            );
                        return obj;
                    });
            }
        }

        public IEnumerable<Entities.EntityBase> GetEntities()
        {
            if (_actions != null)
            {
                _actions.Each(_ => _());
                _actions = null;
            }

            return _company.MakeEnumerable<Entities.EntityBase>()
                .Union(_addresses.Values)
                .Union(_persons.Values)
                .Union(_cars.Values)
                .Union(_routes.Values)
                .Union(_periods.Values)
                ;
        }
    }
}
