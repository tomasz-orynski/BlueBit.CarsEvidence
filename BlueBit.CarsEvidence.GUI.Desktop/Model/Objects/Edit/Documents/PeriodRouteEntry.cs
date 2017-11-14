using AutoMapper;
using BlueBit.CarsEvidence.BL.Repositories;
using BlueBit.CarsEvidence.Commons.Linq;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.View.General.Helpers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Media;

namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit.Documents
{
    [Attributes.EditInDocumentViewAsChild(typeof(Period))]
    [Attributes.EntityType(typeof(BL.Entities.PeriodRouteEntry))]
    [Attributes.ConverterType(typeof(PeriodRouteEntryConverter))]
    public class PeriodRouteEntry :
        EditDocumentObjectChildWithInfoBase
    {
        public ObservableCollection<View.General.Helpers.Day> AllDays { get { return _Period.YearMonthDays; } }
        public ObservableCollection<View.General.Route> AllRoutes { get { return _routes().Items; } }
        public ObservableCollection<View.General.Person> AllPersons { get { return _persons().Items; } }

        private Period _Period;
        [Required]
        public Period Period { get { return _Period; } set { _Set(ref _Period, value); } }

        private Day _Day;
        [Required]
        public Day Day { get { return _Day; } set { _Set(ref _Day, value); } }

        private View.General.Person _Person;
        [Required]
        public View.General.Person Person { get { return _Person; } set { _Set(ref _Person, value); } }

        private View.General.Route _Route;
        [Required]
        public View.General.Route Route { get { return _Route; } set { _Set(ref _Route, value, OnChangeInternal); } }

        private long? _Distance;
        public long? Distance { get { return _Distance; } set { _Set(ref _Distance, value, OnChangeInternal); } }

        public bool DistanceState { 
            get { return _Distance.HasValue; } 
            set { 
                if (value)
                {
                    Distance = _Route != null ? _Route.Distance : 0;
                }
                else
                {
                    Distance = null;
                }
            }
        }
        public long DistanceValue { 
            get { return _Distance ?? 0; } 
            set { Distance = value; } 
        }

        public Brush Colour
        {
            get { return _Day != null && !_Day.IsWeekend ? Brushes.White : Brushes.Yellow; }
        }

        private void OnChangeInternal()
        {
            if (_Route != null && _Distance == default(long))
                Distance = _Route.Distance;

            if (_Period != null) 
                _Period.OnChange();
        }

        private readonly Func<IViewObjects<View.General.Person>> _persons;
        private readonly Func<IViewObjects<View.General.Route>> _routes;

        static PeriodRouteEntry()
        {
            RegisterPropertyDependency<PeriodRouteEntry>()
                .Add(x => x.Colour, x => x.Day)
                .Add(x => x.DistanceState, x => x.Distance)
                .Add(x => x.DistanceValue, x => x.Distance);
        }

        public PeriodRouteEntry(
            Func<IViewObjects<View.General.Person>> persons,
            Func<IViewObjects<View.General.Route>> routes
            )
        {
            _persons = persons;
            _routes = routes;
        }
    }

    public class PeriodRouteEntryConverter :
        EditObjectConverterWithContext<
            Tuple<Period, BL.Entities.Period>, 
            PeriodRouteEntry, BL.Entities.PeriodRouteEntry>
    {
        private readonly Func<IDbRepositories> _repository;
        private readonly Func<IViewObjects<View.General.Person>> _persons;
        private readonly Func<IViewObjects<View.General.Route>> _routes;

        public PeriodRouteEntryConverter(
            Func<IDbRepositories> repository,
            Func<IViewObjects<View.General.Person>> persons,
            Func<IViewObjects<View.General.Route>> routes
            )
        {
            _repository = repository;
            _persons = persons;
            _routes = routes;
        }

        protected override IMappingExpression<BL.Entities.PeriodRouteEntry, PeriodRouteEntry> OnInitialize(IMappingExpression<BL.Entities.PeriodRouteEntry, PeriodRouteEntry> mapingExpr)
        {
            return base.OnInitialize(mapingExpr)
                .ForMember(
                    r => r.Person,
                    cfg => cfg.MapFrom(
                        r => _persons.ConvertByGet(r.Person)
                        ))
                .ForMember(
                    r => r.Route,
                    cfg => cfg.MapFrom(
                        r => _routes.ConvertByGet(r.Route)
                        ))
                .ForMember(
                    r => r.Period,
                    cfg => cfg.Ignore()
                )
                .ForMember(
                    r => r.Day,
                    cfg => cfg.Ignore()
                )
                .ForMember(
                    r => r.DistanceState,
                    cfg => cfg.Ignore()
                )
                .ForMember(
                    r => r.DistanceValue,
                    cfg => cfg.Ignore()
                );
        }

        protected override IMappingExpression<PeriodRouteEntry, BL.Entities.PeriodRouteEntry> OnInitialize(IMappingExpression<PeriodRouteEntry, BL.Entities.PeriodRouteEntry> mapingExpr)
        {
            return base.OnInitialize(mapingExpr)
                .ForMember(
                    r => r.Person,
                    cfg => cfg.MapFrom(
                        r => _repository.ConvertByLoad<BL.Entities.Person>(r.Person.ID)
                        )
                )
                .ForMember(
                    r => r.Route,
                    cfg => cfg.MapFrom(
                        r => _repository.ConvertByLoad<BL.Entities.Route>(r.Route.ID)
                        )
                )
                .ForMember(
                    r => r.Period,
                    cfg => cfg.Ignore()
                )
                .ForMember(
                    r => r.Day,
                    cfg => cfg.MapFrom(
                        r => r.Day.Number
                        )
                );
        }

        protected override void OnCreateUpdate(Tuple<Period, BL.Entities.Period> ctx, BL.Entities.PeriodRouteEntry src, PeriodRouteEntry dst)
        {
            dst.Period = ctx.Item1;
            dst.Day = ctx.Item1.YearMonthDays.SingleOrDefault(_ => _.Number == src.Day);
            if (!dst.IsFromDb())
            {
                dst.Person = dst.AllPersons.OnlyOneOrDefault();
                dst.Route = dst.AllRoutes.OnlyOneOrDefault();
            }
        }

        protected override void OnCreateUpdate(Tuple<Period, BL.Entities.Period> ctx, PeriodRouteEntry src, BL.Entities.PeriodRouteEntry dst)
        {
            dst.Period = ctx.Item2;
        }
    }
}
