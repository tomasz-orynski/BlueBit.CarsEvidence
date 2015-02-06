using BlueBit.CarsEvidence.BL.Entities;
using BlueBit.CarsEvidence.BL.Entities.Components;
using BlueBit.CarsEvidence.BL.Repositories;
using dotNetExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlueBit.CarsEvidence.Console
{
    partial class Program
    {
        static void CreateData()
        {
            using (var sessionFactory = BL.Configuration.Settings.CreateSessionFactory())
            using (var session = sessionFactory.OpenSession())
            {
#if DEBUG
                var persons = CreateData_Persons().ToList();
                var addresses = CreateData_Addresses().ToList();
                var companies = CreateData_Companies(addresses).ToList();
                var routes = CreateData_Routes(addresses).ToList();
                var cars = CreateData_Cars().ToList();
                var periods = CreateData_Periods(cars, persons, routes).ToList();
                var objects = companies.Cast<object>()
                    .Union(persons)
                    .Union(addresses)
                    .Union(routes)
                    .Union(cars)
                    .Union(periods);
#else
                var persons = CreateData_Persons().ToList();
                var addresses = CreateData_Addresses().ToList();
                var companies = CreateData_Companies(addresses).ToList();
                var objects = companies.Cast<object>()
                    .Union(persons)
                    .Union(addresses);
#endif
                using (var transaction = session.BeginTransaction())
                {
                    objects.Each(_ => session.Save(_));
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

        static string CreateInfo(int i = 0)
        {
            var bld = new StringBuilder();
            bld.AppendLine("Przykładowa informacja dla obiektu.");
            bld.AppendLine("Może być wieloliniowa...");
            while (--i >= 0)
                bld.AppendFormat("Dodatkowa linia nr [{0}].", i);
            return bld.ToString();
        }

        static IEnumerable<Person> CreateData_Persons()
        {
            yield return CreateEntity<Person>(_ =>
            { 
                _.Code = "ORYNSKI_T";
                _.FirstName = "Tomasz";
                _.LastName = "Oryński";
            });
#if DEBUG
            yield return CreateEntity<Person>(_ =>
            { 
                _.Code = "ORYNSKI_L";
                _.FirstName = "Łukasz";
                _.LastName = "Oryński";
                _.Info = CreateInfo(5);
            });
#endif
        }

        static IEnumerable<Car> CreateData_Cars()
        {
            foreach (var i in Enumerable.Range(1, 9))
            {
                yield return CreateEntity<Car>(_ =>
                {
                    CounterState beg = null;
                    CounterState end = null;
                    {
                        var t = i % 3;
                        if (t > 0) beg = new CounterState() { Counter = 1000 * i, Date = new DateTime(2014, i, 1) };
                        if (t > 1) end = new CounterState() { Counter = 100000, Date = new DateTime(2050, 12, 31) };
                    }
                    _.Code = string.Format("POJAZD_{0}", i);
                    _.BrandInfo = string.Format("Marka pojazdu {0}", i);
                    _.RegisterNumber = string.Format("WWL000{0}", i);
                    _.EvidenceBegin = beg;
                    _.EvidenceEnd = end;
                    _.Info = CreateInfo(i);
                });
            }
        }

        static IEnumerable<Address> CreateData_Addresses()
        {
            yield return CreateEntity<Address>(_ =>
            { 
                _.Code = "SIEDZIBA";
                _.PostalCode = "05-270";
                _.City = "Marki";
                _.Street = "Zagłoby";
                _.BuildingNo = "53";
            });

#if DEBUG
            foreach (var i in Enumerable.Range(1, 9))
            {
                yield return CreateEntity<Address>(_ =>
                {
                    _.Code = string.Format("KLIENT_{0}", i);
                    _.PostalCode = string.Format("00-00{0}", i);
                    _.City = "Warszawa";
                    _.Street = "Nieznana";
                    _.BuildingNo = string.Format("{0}", i*10);
                    _.Info = CreateInfo(i);
                });
            }
#endif
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
                    _.Info = CreateInfo(i);
                },
                address.AddRouteFrom, addresses[i].AddRouteTo);
            }
        }

        static IEnumerable<Company> CreateData_Companies(List<Address> addresses)
        {
            var address = addresses[0];
            yield return CreateEntity<Company>(_ =>
            {
                _.Code = "BLUE_BIT";
                _.Name = "BLUE BIT Tomasz Oryński";
                _.IdentifierNIP = "773-201-66-36";
                _.IdentifierREGON = "12345678901234";
            },
            address.AddCompany);
        }

        static IEnumerable<Tuple<int, int>> CreateData_PeriodsYM(DateTime dt)
        {
            var year = dt.Year;
            var month = dt.Month;

            do
            {
                yield return Tuple.Create(year, month++);
                if (month > 12)
                {
                    year++;
                    month = 1;
                }
            } while (year < 2016);
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
                if (car.EvidenceBegin == null)
                    continue;

                foreach (var ym in CreateData_PeriodsYM(car.EvidenceBegin.Date))
                {
                    var period = CreateEntity<Period>(_ => {
                            _.Year = ym.Item1;
                            _.Month = (byte)ym.Item2;
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
                                    switch(day%3)
                                    {
                                        case 1:
                                            _.Distance = route.Distance + 1;
                                            break;
                                        case 2:
                                            _.Distance = route.Distance + 2;
                                            break;
                                    }
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
