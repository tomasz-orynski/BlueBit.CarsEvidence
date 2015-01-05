using AutoMapper;
using BlueBit.CarsEvidence.BL.Alghoritms;
using BlueBit.CarsEvidence.BL.Repositories;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.View.General.Helpers;
using BlueBit.CarsEvidence.GUI.Desktop.Services;
using System;
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

        private int _Year;
        [Required]
        [Attributes.Validation.YearRange]
        public int Year { get { return _Year; } set { Set(ref _Year, value, OnChange); } }

        private Month _Month;
        [Required]
        public Month Month { get { return _Month; } set { Set(ref _Month, value, OnChange); } }

        private View.General.Car _Car;
        [Required]
        public View.General.Car Car { get { return _Car; } set { Set(ref _Car, value, OnChange); } }

        private ObservableCollection<PeriodEntry> _Entries;
        [IgnoreMap]
        public ObservableCollection<PeriodEntry> Entries { get { return _Entries; } set { Set(ref _Entries, value, OnChange); } }

        public string Code { get { return string.Format("{0:0000}-{1:00}", _Year, _Month.Number); } }
        public override sealed string DescriptionForTitle { get { return this.GetDescriptionForTitle(); } }

        public ObservableCollection<Day> YearMonthDays 
        { 
            get { 
                var yearMonth = Tuple.Create(_Year, _Month);
                return yearMonth.GetItems(); 
            } 
        }

        private long _DistanceTotal;
        public long DistanceTotal { get { return _DistanceTotal; } private set { Set(ref _DistanceTotal, value); } }

        private long? _CounterBegin;
        public long? CounterBegin { get { return _CounterBegin; } private set { Set(ref _CounterBegin, value); } }

        private long? _CounterEnd;
        public long? CounterEnd { get { return _CounterEnd; } private set { Set(ref _CounterEnd, value); } }

        private readonly Func<IDbRepositories> _repository;
        private readonly Func<IViewObjects<View.General.Car>> _cars;

        public Period(
            Func<IDbRepositories> repository,
            Func<IViewObjects<View.General.Car>> cars
            )
        {
            _repository = repository;
            _cars = cars;
        }

        public PeriodEntry AddToEntries(PeriodEntry periodEntry)
        {
            periodEntry.Period = this;
            periodEntry.ID = Entries.GetTempID();
            Entries.Add(periodEntry);
            return periodEntry;
        }
        public void RemoveFromEntries(PeriodEntry periodEntry)
        {
            Entries.Remove(periodEntry);
        }

        public void OnChange()
        {
            var yearMonthDays = YearMonthDays;
            if (Entries != null)
            {
                var distanceCurr = 0L;

                Entries
                    .Each(entry =>
                    {
                        if (entry.Day != null)
                            entry.Day = yearMonthDays.SingleOrDefault(_ => _.Number == entry.Day.Number);
                        distanceCurr += entry.Distance;
                    });

                if (_Car != null && _Month != null)
                {
                    var distancePrev = _repository().GetTotalDistanceForPrevMonths(_Car.ID, _Year, _Month.Number);
                    CounterBegin = distancePrev;
                    CounterEnd = distancePrev + distanceCurr;
                }
                else
                {
                    CounterBegin = null;
                    CounterEnd = null;
                }
                DistanceTotal = distanceCurr;
            }
        }
    }

    public class PeriodConverter :
        EditObjectConverter<Period, BL.Entities.Period>
    {
        private readonly Func<IDbRepositories> _repository;
        private readonly Func<IEnviromentService> _enviromentService;
        private readonly Func<IViewObjects<View.General.Car>> _cars;
        private readonly Func<IConverterFromEntityChild<Tuple<Period, BL.Entities.Period>, PeriodEntry, BL.Entities.PeriodEntry>> _converterFromEntity;
        private readonly Func<IConverterToEntityChild<Tuple<Period, BL.Entities.Period>, PeriodEntry, BL.Entities.PeriodEntry>> _converterToEntity;

        public PeriodConverter(
            Func<IDbRepositories> repository,
            Func<IEnviromentService> enviromentService,
            Func<IViewObjects<View.General.Car>> cars,
            Func<IConverterFromEntityChild<Tuple<Period, BL.Entities.Period>, PeriodEntry, BL.Entities.PeriodEntry>> converterFromEntity,
            Func<IConverterToEntityChild<Tuple<Period, BL.Entities.Period>, PeriodEntry, BL.Entities.PeriodEntry>> converterToEntity
            )
        {
            _repository = repository;
            _enviromentService = enviromentService;
            _cars = cars;
            _converterFromEntity = converterFromEntity;
            _converterToEntity = converterToEntity;
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
                    r => r.Entries,
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
                    r => r.PeriodEntries,
                    cfg => cfg.Ignore()
                )
                ;
        }

        protected override BL.Entities.Period OnCreate(BL.Entities.Period src)
        {
            var env = _enviromentService();
            src.Year = env.GetCurrentYear();
            src.Month = env.GetCurrentMonth();
            return src;
        }

        protected override void OnCreateUpdate(BL.Entities.Period src, Period dst, Mode mode)
        {
            dst.Entries = _converterFromEntity().Convert(
                dst, src,
                src.PeriodEntries
                );
        }

        protected override void OnCreateUpdate(Period src, BL.Entities.Period dst, Mode mode)
        {
            dst.PeriodEntries = _converterToEntity().Convert(
                src, dst,
                src.Entries,
                dst.PeriodEntries
                );
        }
    }
}
