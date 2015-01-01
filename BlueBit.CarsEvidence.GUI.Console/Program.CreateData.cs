using BlueBit.CarsEvidence.BL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotNetExt;
using BlueBit.CarsEvidence.BL.Repositories;

namespace BlueBit.CarsEvidence.Console
{
    partial class Program
    {
        static void CreateData()
        {
            using (var sessionFactory = BL.Configuration.Settings.CreateSessionFactory(connectionStringKey))
            using (var session = sessionFactory.OpenSession())
            {
                var companies = CreateData_Companies().ToList();
                var persons = CreateData_Persons().ToList();
                var address = CreateData_Addresses().ToList();
                var routes = CreateData_Routes(address).ToList();
                var cars = CreateData_Cars().ToList();
                var periods = CreateData_Periods(cars, persons, routes).ToList();

                using (var transaction = session.BeginTransaction())
                {
                    companies.ForEach(o => session.Save(o));
                    persons.ForEach(o => session.Save(o));
                    address.ForEach(o => session.Save(o));
                    routes.ForEach(o => session.Save(o));
                    cars.ForEach(o => session.Save(o));
                    periods.ForEach(o => session.Save(o));

                    transaction.Commit();
                }
            }
        }

        static T CreateEntity<T>(Action<T> initializtor, params Func<T,T>[] addToFuncs)
            where T : ObjectInRepositoryBase, new()
        {
            var entity = ObjectInRepositoryBase.Create<T>();
            initializtor(entity);
            addToFuncs.Each(f => f(entity));
            return entity;
        }
        static T CreateEntityChild<T>(Action<T> initializtor, params Func<T, T>[] addToFuncs)
            where T : ObjectChildInRepositoryBase, new()
        {
            var entity = ObjectChildInRepositoryBase.Create<T>();
            initializtor(entity);
            addToFuncs.Each(f => f(entity));
            return entity;
        }

        static IEnumerable<Company> CreateData_Companies()
        {
            yield return new Company() { Code = "BLUE_BIT" };
        }

        static IEnumerable<Person> CreateData_Persons()
        {
            yield return CreateEntity<Person>(_ =>
            { 
                _.Code = "ORYNSKI_T";
                _.FirstName = "Tomasz";
                _.LastName = "Oryński";
            });
            yield return CreateEntity<Person>(_ =>
            { 
                _.Code = "ORYNSKI_L";
                _.FirstName = "Łukasz";
                _.LastName = "Oryński";
            });
        }

        static IEnumerable<Car> CreateData_Cars()
        {
            foreach (var i in Enumerable.Range(1, 9))
            {
                yield return CreateEntity<Car>(_ =>
                { 
                    _.Code = string.Format("POJAZD_{0}", i);
                    _.BrandInfo = string.Format("Marka pojazdu {0}", i);
                    _.RegisterNumber = string.Format("WWL000{0}", i);
                    _.EvidenceCounterBegin = 1000*i;
                    _.EvidenceDateBegin = new DateTime(2014, i, 1);
                });
            }
        }

        static IEnumerable<Address> CreateData_Addresses()
        {
            yield return CreateEntity<Address>(_ =>
            { 
                _.Code = "SIEDZIBA";
                _.PostalCode = "01-234";
                _.City = "Marki";
                _.Street = "Centralna";
                _.BuildingNo = "1234";
                _.LocalNo = "ABC";
            });

            foreach (var i in Enumerable.Range(1, 9))
            {
                yield return CreateEntity<Address>(_ =>
                {
                    _.Code = string.Format("KLIENT_{0}", i);
                    _.PostalCode = string.Format("00-00{0}", i);
                    _.City = "Warszawa";
                    _.Street = "Nieznana";
                    _.BuildingNo = string.Format("{0}", i*10);
                });
            }
        }

        static IEnumerable<Route> CreateData_Routes(List<Address> addresses)
        {
            var address = addresses[0];
            for (var i = 1; i < addresses.Count; i++)
            {
                yield return CreateEntity<Route>(_ =>
                {
                    _.Code = string.Format("{0}-{1}", address.Code, addresses[i].Code);
                    _.Distance = i * 10;
                    _.DistanceIsInBothDirections = (i % 2) == 0;
                },
                address.AddRouteFrom, addresses[i].AddRouteTo);
            }
        }

        static IEnumerable<Period> CreateData_Periods(
            List<Car> cars,
            List<Person> persons,
            List<Route> routes)
        {
            var routeIdx = 0;
            var personIdx = 0;

            Func<Route> getNextRoute = () =>
                {
                    if (routeIdx == routes.Count) routeIdx = 0;
                    return routes[routeIdx++];
                };
            Func<Person> getNextPerson = () =>
            {
                if (personIdx == persons.Count) personIdx = 0;
                return persons[personIdx++];
            };

            foreach (var car in cars)
            {
                var dt = car.EvidenceDateBegin;
                for (var i = dt.Month; i <= 12; i++)
                {
                    var period = CreateEntity<Period>(_ => {
                            _.Year = dt.Year;
                            _.Month = (byte)i;
                        },
                        car.AddPeriod
                    );

                    var day = 1;
                    while (true)
                    {
                        var dayDate = new DateTime(period.Year, period.Month, day);

                        if (dayDate.DayOfWeek >= DayOfWeek.Monday && dayDate.DayOfWeek <= DayOfWeek.Saturday)
                        {
                            var route = getNextRoute();
                            var person = getNextPerson();

                            var periodEntry = CreateEntityChild<PeriodEntry>(_ => {
                                    _.Day = (byte)day;
                                },
                                period.AddPeriodEntry, person.AddPeriodEntry, route.AddPeriodEntry
                            );
                            if (routeIdx == routes.Count) routeIdx = 0;
                        }

                        var nextDayDate = dayDate + TimeSpan.FromDays(1);
                        if (nextDayDate.Month != period.Month) break;
                        ++day;
                    }

                    yield return period;
                }
            }
        }
    }
}
