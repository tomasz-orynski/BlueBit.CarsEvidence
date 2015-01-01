using AutoMapper;
using BlueBit.CarsEvidence.BL.Repositories;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Attributes.Validation;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.View;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.View.General.Helpers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Media;

namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit.Documents
{
    [Attributes.EditInDocumentViewAsChild(typeof(Period))]
    [Attributes.EntityType(typeof(BL.Entities.PeriodEntry))]
    [Attributes.ConverterType(typeof(PeriodEntryConverter))]
    public class PeriodEntry :
        EditObjectBase
    {
        public ObservableCollection<View.General.Helpers.Day> AllDays { get { return _Period.YearMonthDays; } }
        public ObservableCollection<View.General.Route> AllRoutes { get { return _routes().Items; } }
        public ObservableCollection<View.General.Person> AllPersons { get { return _persons().Items; } }

        private Period _Period;
        [Required]
        public Period Period { get { return _Period; } set { Set(ref _Period, value); } }

        private Day _Day;
        [Required]
        public Day Day { get { return _Day; } set { Set(ref _Day, value); } }

        private View.General.Person _Person;
        [Required]
        public View.General.Person Person { get { return _Person; } set { Set(ref _Person, value); } }

        private View.General.Route _Route;
        [Required]
        public View.General.Route Route { get { return _Route; } set { Set(ref _Route, value, OnChangeRoute); } }

        private long _Distance;
        [Required]
        public long Distance { get { return _Distance; } set { Set(ref _Distance, value); } }

        public Brush Colour
        {
            get { return _Day != null && !_Day.IsWeekend ? Brushes.White : Brushes.Yellow; }
        }

        private void OnChangeRoute()
        {
            if (_Route != null && _Distance == default(long))
                Distance = _Route.Distance;

            if (_Period != null) 
                _Period.OnChange();
        }

        private readonly Func<IViewObjects<View.General.Person>> _persons;
        private readonly Func<IViewObjects<View.General.Route>> _routes;

        static PeriodEntry()
        {
            RegisterPropertyDependency<PeriodEntry>()
                .Add(x => x.Colour, x => x.Day);
        }

        public PeriodEntry(
            Func<IViewObjects<View.General.Person>> persons,
            Func<IViewObjects<View.General.Route>> routes
            )
        {
            _persons = persons;
            _routes = routes;
        }
    }

    public class PeriodEntryConverter :
        EditObjectConverterWithContext<
            Tuple<Period, BL.Entities.Period>, 
            PeriodEntry, BL.Entities.PeriodEntry>
    {
        private readonly Func<IDbRepositories> _repository;
        private readonly Func<IViewObjects<View.General.Person>> _persons;
        private readonly Func<IViewObjects<View.General.Route>> _routes;

        public PeriodEntryConverter(
            Func<IDbRepositories> repository,
            Func<IViewObjects<View.General.Person>> persons,
            Func<IViewObjects<View.General.Route>> routes
            )
        {
            _repository = repository;
            _persons = persons;
            _routes = routes;
        }

        protected override IMappingExpression<BL.Entities.PeriodEntry, PeriodEntry> OnInitialize(IMappingExpression<BL.Entities.PeriodEntry, PeriodEntry> mapingExpr)
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
                );
        }

        protected override IMappingExpression<PeriodEntry, BL.Entities.PeriodEntry> OnInitialize(IMappingExpression<PeriodEntry, BL.Entities.PeriodEntry> mapingExpr)
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

        protected override void OnCreateUpdate(Tuple<Period, BL.Entities.Period> ctx, BL.Entities.PeriodEntry src, PeriodEntry dst)
        {
            dst.Period = ctx.Item1;
            dst.Day = ctx.Item1.YearMonthDays.SingleOrDefault(_ => _.Number == src.Day);
        }

        protected override void OnCreateUpdate(Tuple<Period, BL.Entities.Period> ctx, PeriodEntry src, BL.Entities.PeriodEntry dst)
        {
            dst.Period = ctx.Item2;
        }
    }
}
