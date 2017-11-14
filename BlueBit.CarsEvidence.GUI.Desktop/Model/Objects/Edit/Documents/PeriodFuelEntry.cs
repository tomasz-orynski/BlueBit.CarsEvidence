using AutoMapper;
using BlueBit.CarsEvidence.BL.Entities.Components;
using BlueBit.CarsEvidence.BL.Entities.Enums;
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
    [Attributes.EntityType(typeof(BL.Entities.PeriodFuelEntry))]
    [Attributes.ConverterType(typeof(PeriodFuelEntryConverter))]
    public class PeriodFuelEntry :
        EditDocumentObjectChildWithInfoBase
    {
        public ObservableCollection<View.General.Helpers.Day> AllDays { get { return _Period.YearMonthDays; } }
        public ObservableCollection<EV<TimeOfDay>> AllTimeOfDays { get { return EVExtensions<TimeOfDay>.Items; } }
        public ObservableCollection<View.General.Person> AllPersons { get { return _persons().Items; } }
        public ObservableCollection<EV<FuelType>> AllPurchaseTypes { get { return EVExtensions<FuelType>.Items; } }

        private Period _Period;
        [Required]
        public Period Period { get { return _Period; } set { _Set(ref _Period, value); } }

        private Day _Day;
        [Required]
        public Day Day { get { return _Day; } set { _Set(ref _Day, value); } }

        private EV<TimeOfDay> _TimeOfDay;
        [Required]
        public EV<TimeOfDay> TimeOfDay { get { return _TimeOfDay; } set { _Set(ref _TimeOfDay, value); } }

        private View.General.Person _Person;
        [Required]
        public View.General.Person Person { get { return _Person; } set { _Set(ref _Person, value); } }

        private EV<FuelType> _PurchaseType;
        [Required]
        public EV<FuelType> PurchaseType { get { return _PurchaseType; } set { _Set(ref _PurchaseType, value); } }

        private decimal _PurchaseVolume;
        [Required]
        public decimal PurchaseVolume { get { return _PurchaseVolume; } set { _Set(ref _PurchaseVolume, value, OnChangeInternal); } }

        private decimal _PurchaseAmount;
        [Required]
        public decimal PurchaseAmount { get { return _PurchaseAmount; } set { _Set(ref _PurchaseAmount, value, OnChangeInternal); } }


        public Brush Colour
        {
            get { return _Day != null && !_Day.IsWeekend ? Brushes.White : Brushes.Yellow; }
        }


        private void OnChangeInternal()
        {
            if (_Period != null)
                _Period.OnChange();
        }

        private readonly Func<IViewObjects<View.General.Person>> _persons;

        static PeriodFuelEntry()
        {
            RegisterPropertyDependency<PeriodRouteEntry>()
                .Add(x => x.Colour, x => x.Day);
        }

        public PeriodFuelEntry(
            Func<IViewObjects<View.General.Person>> persons
            )
        {
            _persons = persons;
        }
    }

    public class PeriodFuelEntryConverter :
        EditObjectConverterWithContext<
            Tuple<Period, BL.Entities.Period>,
            PeriodFuelEntry, BL.Entities.PeriodFuelEntry>
    {
        private readonly Func<IDbRepositories> _repository;
        private readonly Func<IViewObjects<View.General.Person>> _persons;

        public PeriodFuelEntryConverter(
            Func<IDbRepositories> repository,
            Func<IViewObjects<View.General.Person>> persons
            )
        {
            _repository = repository;
            _persons = persons;
        }

        protected override IMappingExpression<BL.Entities.PeriodFuelEntry, PeriodFuelEntry> OnInitialize(IMappingExpression<BL.Entities.PeriodFuelEntry, PeriodFuelEntry> mapingExpr)
        {
            return base.OnInitialize(mapingExpr)
                .ForMember(
                    r => r.Person,
                    cfg => cfg.MapFrom(
                        r => _persons.ConvertByGet(r.Person)
                    )
                )
                .ForMember(
                    r => r.TimeOfDay,
                    cfg => cfg.MapFrom(
                        r => EVExtensions<TimeOfDay>.GetItem(r.TimeOfDay)
                    )
                )
                .ForMember(
                    r => r.PurchaseType,
                    cfg => cfg.MapFrom(
                        r => EVExtensions<FuelType>.GetItem(r.Purchase.Type)
                    )
                )
                .ForMember(
                    r => r.Period,
                    cfg => cfg.Ignore()
                )
                .ForMember(
                    r => r.Day,
                    cfg => cfg.Ignore()
                );
        }

        protected override IMappingExpression<PeriodFuelEntry, BL.Entities.PeriodFuelEntry> OnInitialize(IMappingExpression<PeriodFuelEntry, BL.Entities.PeriodFuelEntry> mapingExpr)
        {
            return base.OnInitialize(mapingExpr)
                .ForMember(
                    r => r.Person,
                    cfg => cfg.MapFrom(
                        r => _repository.ConvertByLoad<BL.Entities.Person>(r.Person.ID)
                        )
                )
                .ForMember(
                    r => r.TimeOfDay,
                    cfg => cfg.MapFrom(
                        r => r.TimeOfDay.Value
                    )
                )
                .ForMember(
                    r => r.Purchase,
                    cfg => cfg.MapFrom(
                        r => new FuelPurchase()
                        {
                            Type = r.PurchaseType.Value,
                            Amount = r.PurchaseAmount,
                            Volume = r.PurchaseVolume,
                        }
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

        protected override void OnCreateUpdate(Tuple<Period, BL.Entities.Period> ctx, BL.Entities.PeriodFuelEntry src, PeriodFuelEntry dst)
        {
            dst.Period = ctx.Item1;
            dst.Day = ctx.Item1.YearMonthDays.SingleOrDefault(_ => _.Number == src.Day);
            if (!dst.IsFromDb())
            {
                dst.Person = dst.AllPersons.OnlyOneOrDefault();
            }
        }

        protected override void OnCreateUpdate(Tuple<Period, BL.Entities.Period> ctx, PeriodFuelEntry src, BL.Entities.PeriodFuelEntry dst)
        {
            dst.Period = ctx.Item2;
        }
    }
}
