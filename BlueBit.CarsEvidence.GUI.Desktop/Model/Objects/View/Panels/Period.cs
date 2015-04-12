using AutoMapper;
using BlueBit.CarsEvidence.BL.Entities.Components;
using BlueBit.CarsEvidence.Commons.Templates;
using System;

namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.View.Panels
{
    [Attributes.ShowInPanelView]
    [Attributes.EntityType(typeof(BL.Entities.Period))]
    [Attributes.ConverterType(typeof(PeriodConverter))]
    public class Period :
        ViewPanelObjectBase,
        IObjectWithGetCode
    {
        private int _Year;
        public int Year { get { return _Year; } set { _Set(ref _Year, value); } }

        private int _Month;
        public int Month { get { return _Month; } set { _Set(ref _Month, value); } }

        private General.Car _Car;
        public General.Car Car { get { return _Car; } set { _Set(ref _Car, value); } }

        private IValueStatsBase<long> _RouteStats;
        public virtual IValueStatsBase<long> RouteStats { get { return _RouteStats; } set { _Set(ref _RouteStats, value); } }

        private IPurchaseStatsBase _FuelStats;
        public virtual IPurchaseStatsBase FuelStats { get { return _FuelStats; } set { _Set(ref _FuelStats, value); } }

        public string Code { get { return string.Format("{0:0000}-{1:00}", _Year, _Month); } }

        public string Description { get { return this.GetDescription(); } }
        public sealed override string DescriptionForToolTip { get { return this.GetDescription(); } }

        static Period()
        {
            RegisterPropertyDependency<Period>()
                .Add(x => x.Code, x => x.Year, x => x.Month)
                .Add(x => x.Description, x => x.Year, x => x.Month)
                .Add(x => x.DescriptionForToolTip, x => x.Year, x => x.Month);
        }
    }

    public class PeriodConverter :
        ViewObjectConverter<Period, BL.Entities.Period>
    {
        private readonly Func<IGeneralObjects<View.General.Car>> _cars;

        public PeriodConverter(
            Func<IGeneralObjects<View.General.Car>> cars
            )
        {
            _cars = cars;
        }

        protected override void OnInitialize(IMappingExpression<BL.Entities.Period, Period> mapingExpr)
        {
            mapingExpr
                .ForMember(
                    r => r.Car,
                    cfg => cfg.MapFrom(
                        r => _cars.ConvertByGet(r.Car)
                        ))
            ;
        }
    }
}
