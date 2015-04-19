using AutoMapper;
using Entities = BlueBit.CarsEvidence.BL.Entities;
using BlueBit.CarsEvidence.BL.Repositories;
using BlueBit.CarsEvidence.Commons.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Diagnostics.Contracts;
using BlueBit.CarsEvidence.BL.Entities.Enums;
using BlueBit.CarsEvidence.BL.Entities.Components;
using BlueBit.CarsEvidence.BL.Alghoritms;

namespace BlueBit.CarsEvidence.BL.DTO.XML
{
    [DataContract(Namespace = Consts.Namespace_DTO_XML)]
    public class ValueState<T>
    {
        [DataMember(Order = 1)]
        public DateTime Date { get; set; }
        [DataMember(Order = 2)]
        public long Value { get; set; }
    }

    [DataContract(Namespace = Consts.Namespace_DTO_XML)]
    public class FuelPurchase
    {
        [DataMember(Order = 1)]
        public FuelType Type { get; set; }
        [DataMember(Order = 2)]
        public decimal Volume { get; set; }
        [DataMember(Order = 3)]
        public decimal Amount { get; set; }
    }

    [DataContract(Namespace = Consts.Namespace_DTO_XML)]
    public class ValueStateLong : ValueState<long>
    {
    }

    [DataContract(Namespace = Consts.Namespace_DTO_XML)]
    public abstract class DTOBase 
    {
        [DataMember(Order = 0)]
        public long ID { get; set; }
    }

    [DataContract(Namespace = Consts.Namespace_DTO_XML)]
    public abstract class DTOWithCodeAndInfoBase :
        DTOBase
    {
        [DataMember(Order = 1)]
        public string Code { get; set; }
        [DataMember(Order = 2)]
        public string Info { get; set; }
    }

    [DataContract(Namespace = Consts.Namespace_DTO_XML)]
    public class Address :
        DTOWithCodeAndInfoBase
    {
        [DataMember(Order = 12)]
        public string PostalCode { get; set; }
        [DataMember(Order = 11)]
        public string City { get; set; }
        [DataMember(Order = 13)]
        public string Street { get; set; }
        [DataMember(Order = 10)]
        public string BuildingNo { get; set; }
        [DataMember(Order = 11)]
        public string LocalNo { get; set; }
    }

    [DataContract(Namespace = Consts.Namespace_DTO_XML)]
    public class Company :
        DTOWithCodeAndInfoBase
    {
        [DataMember(Order = 13)]
        public string Name { get; set; }
        [DataMember(Order = 11)]
        public string IdentifierNIP { get; set; }
        [DataMember(Order = 12)]
        public string IdentifierREGON { get; set; }
        [DataMember(Order = 10)]
        public long AddressID { get; set; }
    }

    [DataContract(Namespace = Consts.Namespace_DTO_XML)]
    public class Car :
        DTOWithCodeAndInfoBase
    {
        [DataMember(Order = 13)]
        public string RegisterNumber { get; set; }
        [DataMember(Order = 10)]
        public string BrandInfo { get; set; }
        [DataMember(Order = 11)]
        public ValueStateLong EvidenceBeg { get; set; }
        [DataMember(Order = 12)]
        public ValueStateLong EvidenceEnd { get; set; }
    }

    [DataContract(Namespace = Consts.Namespace_DTO_XML)]
    public class Person :
        DTOWithCodeAndInfoBase
    {
        [DataMember(Order = 10)]
        public string FirstName { get; set; }
        [DataMember(Order = 11)]
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
        public PeriodRouteEntry[] RouteEntries { get; set; }
        [DataMember(Order = 21)]
        public PeriodFuelEntry[] FuelEntries { get; set; }
    }

    [DataContract(Namespace = Consts.Namespace_DTO_XML)]
    public class PeriodRouteEntry :
        DTOBase
    {
        [DataMember(Order = 10)]
        public byte Day { get; set; }
        [DataMember(Order = 12)]
        public long PersonID { get; set; }
        [DataMember(Order = 13)]
        public long RouteID { get; set; }
        [DataMember(Order = 11)]
        public long? Distance { get; set; }
    }

    [DataContract(Namespace = Consts.Namespace_DTO_XML)]
    public class PeriodFuelEntry :
        DTOBase
    {
        [DataMember(Order = 10)]
        public byte Day { get; set; }
        [DataMember(Order = 11)]
        public TimeOfDay TimeOfDay { get; set; }
        [DataMember(Order = 12)]
        public long PersonID { get; set; }
        [DataMember(Order = 13)]
        public FuelPurchase Purchase { get; set; }
    }

    [DataContract(Namespace = Consts.Namespace_DTO_XML)]
    public class Route :
        DTOWithCodeAndInfoBase
    {
        [DataMember(Order = 10)]
        public long AddressFromID { get; set; }
        [DataMember(Order = 11)]
        public long AddressToID { get; set; }
        [DataMember(Order = 12)]
        public long Distance { get; set; }
        [DataMember(Order = 13)]
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
            Mapper.CreateMap<Entities.Components.ValueState<long>, ValueStateLong>();
            Mapper.CreateMap<Entities.Components.FuelPurchase, FuelPurchase>();
            Mapper.CreateMap<Entities.Address, Address>();
            Mapper.CreateMap<Entities.Car, Car>();
            Mapper.CreateMap<Entities.Company, Company>();
            Mapper.CreateMap<Entities.Person, Person>();
            Mapper.CreateMap<Entities.Period, Period>()
                .ForSourceMember(
                    src => src.RouteStats,
                    cfg => cfg.Ignore())
                .ForSourceMember(
                    src => src.FuelStats,
                    cfg => cfg.Ignore());
            Mapper.CreateMap<Entities.PeriodRouteEntry, PeriodRouteEntry>();
            Mapper.CreateMap<Entities.PeriodFuelEntry, PeriodFuelEntry>();
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

        private TDTO[] GetAllInArray<TEntity, TDTO>()
            where TEntity : class, IObjectInRepository
            where TDTO: DTOWithCodeAndInfoBase
        {
            return GetAll<TEntity, TDTO>()
                .OrderBy(_ => _.Code)
                .ToArray();
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
        public Person[] Persons { get { return GetAllInArray<Entities.Person, Person>(); } }
        [DataMember(Order = 2)]
        public Car[] Cars { get { return GetAllInArray<Entities.Car, Car>(); } }
        [DataMember(Order = 3)]
        public Address[] Addresses { get { return GetAllInArray<Entities.Address, Address>(); } }
        [DataMember(Order = 4)]
        public Route[] Routes { get { return GetAllInArray<Entities.Route, Route>(); } }
        [DataMember(Order = 5)]
        public Period[] Periods { get { return GetAll<Entities.Period, Period>().OrderBy(_ => Tuple.Create(_.Year, _.Month)).ToArray(); } }
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
            Mapper.CreateMap<ValueStateLong, Entities.Components.ValueState<long>>();
            Mapper.CreateMap<FuelPurchase, Entities.Components.FuelPurchase>();
            PrepareMap<Address, Entities.Address>();
            PrepareMap<Car, Entities.Car>();
            PrepareMap<Company, Entities.Company>();
            PrepareMap<Person, Entities.Person>();
            PrepareMap<Period, Entities.Period>()
                .ForMember(
                    dst => dst.RouteEntries,
                    cfg => cfg.Ignore())
                .ForMember(
                    dst => dst.FuelEntries,
                    cfg => cfg.Ignore());
            PrepareMap<PeriodRouteEntry, Entities.PeriodRouteEntry>();
            PrepareMap<PeriodFuelEntry, Entities.PeriodFuelEntry>();
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
            Contract.Assert(srcID > 0);

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
                        _.RouteEntries.NullAsEmpty().Each(__ =>
                            {
                                var objChild = Mapper.Map<PeriodRouteEntry, Entities.PeriodRouteEntry>(__);
                                Entities.PeriodExtensions.AddRouteEntry(obj, objChild);
                                AddActionForDict(
                                    () => _persons, __.PersonID,
                                    (i) => Entities.PersonExtensions.AddPeriodRouteEntry(i, objChild));
                                AddActionForDict(
                                    () => _routes, __.RouteID,
                                    (i) => Entities.RouteExtensions.AddPeriodRouteEntry(i, objChild));
                            });
                        _.FuelEntries.NullAsEmpty().Each(__ =>
                        {
                            var objChild = Mapper.Map<PeriodFuelEntry, Entities.PeriodFuelEntry>(__);
                            Entities.PeriodExtensions.AddFuelEntry(obj, objChild);
                            AddActionForDict(
                                () => _persons, __.PersonID,
                                (i) => Entities.PersonExtensions.AddPeriodFuelEntry(i, objChild));
                        });
                        AddActions(
                            () => Entities.CarExtensions.AddPeriod(_cars[_.CarID], obj)
                            );
                        return obj;
                    });
            }
        }

        public IEnumerable<IEnumerable<Entities.EntityBase>> GetEntities()
        {
            if (_actions != null)
            {
                _actions.Each(_ => _());
                _actions = null;
            }

            yield return _company.MakeEnumerable<Entities.EntityBase>()
                .Union(_addresses.Values.OrderBy(_ => _.Code))
                .Union(_persons.Values.OrderBy(_ => _.Code))
                .Union(_cars.Values.OrderBy(_ => _.Code))
                .Union(_routes.Values.OrderBy(_ => _.Code));

            foreach (var period in _periods.Values.RecalculateStats().OrderBy(_ => _.Car.Code))
                yield return period.MakeEnumerable();
        }

    }
}
