using AutoMapper;
using BlueBit.CarsEvidence.BL.Entities.Components;
using BlueBit.CarsEvidence.BL.Repositories;
using BlueBit.CarsEvidence.Commons.Helpers;
using BlueBit.CarsEvidence.Commons.Linq;
using BlueBit.CarsEvidence.Commons.Templates;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Components;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.View.General.Helpers;
using BlueBit.CarsEvidence.GUI.Desktop.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit.Documents
{
    [Attributes.EditInDocumentView]
    [Attributes.EntityType(typeof(BL.Entities.Period))]
    [Attributes.ConverterType(typeof(PeriodConverter))]
    public class Period :
        EditDocumentObjectBase,
        IObjectWithGetCode
    {
        public ObservableCollection<Month> AllMonths { get { return MonthExtensions.Items; } }
        public ObservableCollection<View.General.Car> AllCars { get { return _cars().Items; } }

        private string _Info;
        [MaxLength(BL.Configuration.Consts.LengthInfo)]
        public string Info { get { return _Info; } set { _Set(ref _Info, value); } }

        private int _Year;
        [Required]
        [Attributes.Validation.YearRange]
        public int Year { get { return _Year; } set { _Set(ref _Year, value, OnChangeInternal); } }

        private Month _Month;
        [Required]
        public Month Month { get { return _Month; } set { _Set(ref _Month, value, OnChangeInternal); } }

        private View.General.Car _Car;
        [Required]
        public View.General.Car Car { get { return _Car; } set { _Set(ref _Car, value, OnChangeInternal); } }

        private readonly ValueStatsContainer _RouteStats = new ValueStatsContainer();
        public virtual ValueStatsContainer RouteStats { get { return _RouteStats; } }

        private readonly PurchaseStatsContainer _FuelStats = new PurchaseStatsContainer();
        public virtual PurchaseStatsContainer FuelStats { get { return _FuelStats; } }

        private ObservableCollection<PeriodRouteEntry> _RouteEntries;
        [IgnoreMap]
        public ObservableCollection<PeriodRouteEntry> RouteEntries { get { return _RouteEntries; } set { _SetColl(ref _RouteEntries, value, OnChange); } }

        private ObservableCollection<PeriodFuelEntry> _FuelEntries;
        [IgnoreMap]
        public ObservableCollection<PeriodFuelEntry> FuelEntries { get { return _FuelEntries; } set { _SetColl(ref _FuelEntries, value, OnChange); } }

        public string Code { get { return string.Format("{0:0000}-{1:00}", _Year, _Month.Number); } }
        public override sealed string Description { get { return this.GetDescription(); } }

        public ObservableCollection<Day> YearMonthDays 
        { 
            get { 
                var yearMonth = Tuple.Create(_Year, _Month);
                return yearMonth.GetItems(); 
            } 
        }

        private readonly Func<IDbRepositories> _repository;
        private readonly Func<IViewObjects<View.General.Car>> _cars;

        private readonly IEnumerable<Func<bool>> onChangePrevPeriodNeedInvalidate;
        private BL.Entities.Period onChangePrevPeriod = null;

        public Period(
            Func<IDbRepositories> repository,
            Func<IViewObjects<View.General.Car>> cars
            )
        {
            _repository = repository;
            _cars = cars;

            onChangePrevPeriodNeedInvalidate = new Func<bool>[] 
            { 
                () => _Car == null || _Month == null,
                () => _Car.ID != onChangePrevPeriod.Car.ID,
                () => _Year != onChangePrevPeriod.Year,
                () => _Month.Number != onChangePrevPeriod.Month,
            };        
        }

        public PeriodRouteEntry AddToEntries(PeriodRouteEntry periodEntry)
        {
            periodEntry.Period = this;
            periodEntry.ID = RouteEntries.GetTempID();
            RouteEntries.Add(periodEntry);
            return periodEntry;
        }
        public void RemoveFromEntries(PeriodRouteEntry periodEntry)
        {
            RouteEntries.Remove(periodEntry);
        }
        public PeriodFuelEntry AddToEntries(PeriodFuelEntry fuelEntry)
        {
            fuelEntry.Period = this;
            fuelEntry.ID = FuelEntries.GetTempID();
            FuelEntries.Add(fuelEntry);
            return fuelEntry;
        }
        public void RemoveFromEntries(PeriodFuelEntry fuelEntry)
        {
            FuelEntries.Remove(fuelEntry);
        }


        private void OnChangeInternal()
        {
            if (onChangePrevPeriod != null && onChangePrevPeriodNeedInvalidate.Any(_ => _()))
                onChangePrevPeriod = null;
            OnChange();
        }

        public void OnChange()
        {
            if (onChangePrevPeriod == null && _Car != null && _Month != null)
                onChangePrevPeriod = _repository().GetPreviousPeriod(_Car.ID, _Year, _Month.Number);

            recalculateRouteStats(onChangePrevPeriod);
            recalculateFuelStats(onChangePrevPeriod);
        }

        private void recalculateRouteStats(BL.Entities.Period prevPeriod)
        {
            RouteStats.Item = ValueStatsExt.CreateFrom(
                _RouteEntries.NullAsEmpty().Select(entry => entry.Distance ?? entry.Route.GetSafeValue(_ => _.Distance)),
                prevPeriod.GetSafeValue(_ => _.RouteStats.ValueEnd));
        }
        private void recalculateFuelStats(BL.Entities.Period prevPeriod)
        {
            FuelStats.Item = PurchaseStatsExt.CreateFrom(
                _FuelEntries.NullAsEmpty(), _ => _.PurchaseVolume, _ => _.PurchaseAmount,
                prevPeriod.GetSafeValue(_ => _.FuelStats.VolumeEnd),
                prevPeriod.GetSafeValue(_ => _.FuelStats.AmountEnd));
        }
    }

    public class PeriodConverter :
        EditObjectConverter<Period, BL.Entities.Period>
    {
        private readonly Func<IDbRepositories> _repository;
        private readonly Func<IEnviromentService> _enviromentService;
        private readonly Func<IViewObjects<View.General.Car>> _cars;
        private readonly Func<IConverterFromEntityChild<Tuple<Period, BL.Entities.Period>, PeriodFuelEntry, BL.Entities.PeriodFuelEntry>> _converterFromFuelEntity;
        private readonly Func<IConverterToEntityChild<Tuple<Period, BL.Entities.Period>, PeriodFuelEntry, BL.Entities.PeriodFuelEntry>> _converterToFuelEntity;
        private readonly Func<IConverterFromEntityChild<Tuple<Period, BL.Entities.Period>, PeriodRouteEntry, BL.Entities.PeriodRouteEntry>> _converterFromRouteEntity;
        private readonly Func<IConverterToEntityChild<Tuple<Period, BL.Entities.Period>, PeriodRouteEntry, BL.Entities.PeriodRouteEntry>> _converterToRouteEntity;

        public PeriodConverter(
            Func<IDbRepositories> repository,
            Func<IEnviromentService> enviromentService,
            Func<IViewObjects<View.General.Car>> cars,
            Func<IConverterFromEntityChild<Tuple<Period, BL.Entities.Period>, PeriodFuelEntry, BL.Entities.PeriodFuelEntry>> converterFromFuelEntity,
            Func<IConverterToEntityChild<Tuple<Period, BL.Entities.Period>, PeriodFuelEntry, BL.Entities.PeriodFuelEntry>> converterToFuelEntity,
            Func<IConverterFromEntityChild<Tuple<Period, BL.Entities.Period>, PeriodRouteEntry, BL.Entities.PeriodRouteEntry>> converterFromRouteEntity,
            Func<IConverterToEntityChild<Tuple<Period, BL.Entities.Period>, PeriodRouteEntry, BL.Entities.PeriodRouteEntry>> converterToRouteEntity
            )
        {
            _repository = repository;
            _enviromentService = enviromentService;
            _cars = cars;
            _converterFromFuelEntity = converterFromFuelEntity;
            _converterToFuelEntity = converterToFuelEntity;
            _converterFromRouteEntity = converterFromRouteEntity;
            _converterToRouteEntity = converterToRouteEntity;
        }

        protected override void OnInitialize(IMappingExpression<BL.Entities.Period, Period> mapingExpr)
        {
            mapingExpr
                .ForMember(
                    r => r.Month,
                    cfg => cfg.MapFrom(
                        r => r.Month.GetMonth()
                        )
                )
                .ForMember(
                    r => r.Car,
                    cfg => cfg.MapFrom(
                        r => _cars.ConvertByGet(r.Car)
                        )
                )
                .ForMember(
                    r => r.RouteEntries,
                    cfg => cfg.Ignore()
                )
                .ForMember(
                    r => r.FuelEntries,
                    cfg => cfg.Ignore()
                )
                .IgnoreAllPropertiesWithAnInaccessibleSetter()
            ;
        }

        protected override void OnInitialize(IMappingExpression<Period, BL.Entities.Period> mapingExpr)
        {
            mapingExpr
                .ForMember(
                    r => r.Month,
                    cfg => cfg.MapFrom(
                        r => r.Month.Number
                        )
                )
                .ForMember(
                    r => r.Car,
                    cfg => cfg.MapFrom(
                        r => _repository.ConvertByLoad<BL.Entities.Car>(r.Car.ID)
                        )
                )
                .ForMember(
                    r => r.RouteEntries,
                    cfg => cfg.Ignore()
                )
                .ForMember(
                    r => r.FuelEntries,
                    cfg => cfg.Ignore()
                )
                ;
        }

        protected override void OnAfterMap(BL.Entities.Period src, Period dst, Mode mode)
        {
            if (!dst.IsFromDb())
            {
                var env = _enviromentService();
                dst.Year = env.GetCurrentYear();
                dst.Month = dst.AllMonths.Single(_ => _.Number == env.GetCurrentMonth());
                dst.Car = dst.AllCars.OnlyOneOrDefault();
            }

            dst.FuelEntries = _converterFromFuelEntity().Convert(
                dst, src,
                src.FuelEntries
                );
            dst.FuelStats.Item = src.FuelStats;
            dst.RouteEntries = _converterFromRouteEntity().Convert(
                dst, src,
                src.RouteEntries
                );
            dst.RouteStats.Item = src.RouteStats;
        }

        protected override void OnAfterMap(Period src, BL.Entities.Period dst, Mode mode)
        {
            dst.FuelEntries = _converterToFuelEntity().Convert(
                src, dst,
                src.FuelEntries,
                dst.FuelEntries
                );
            dst.FuelStats = src.FuelStats.Item;
            dst.RouteEntries = _converterToRouteEntity().Convert(
                src, dst,
                src.RouteEntries,
                dst.RouteEntries
                );
            dst.RouteStats = src.RouteStats.Item;
        }
    }
}
