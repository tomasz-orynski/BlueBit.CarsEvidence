using AutoMapper;
using System;
using System.Linq;
using BlueBit.CarsEvidence.BL.Alghoritms;
using System.Text;

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
        public int Year { get { return _Year; } set { Set(ref _Year, value); } }

        private int _Month;
        public int Month { get { return _Month; } set { Set(ref _Month, value); } }

        private General.Car _Car;
        public General.Car Car { get { return _Car; } set { Set(ref _Car, value); } }

        private long _DistanceTotal;
        public long DistanceTotal { get { return _DistanceTotal; } set { Set(ref _DistanceTotal, value); } }

        public string Code { get { return string.Format("{0:0000}-{1:00}", _Year, _Month); } }

        public string Description { get { return this.GetDescription(); } }
        public sealed override string DescriptionForToolTip { get { return this.GetDescriptionForToolTip(); } }

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
