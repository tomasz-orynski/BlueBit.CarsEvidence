using AutoMapper;
using BlueBit.CarsEvidence.BL.Alghoritms;
using System;

namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.View.Panels
{
    [Attributes.ShowInPanelView]
    [Attributes.EntityType(typeof(BL.Entities.Route))]
    [Attributes.ConverterType(typeof(RouteConverter))]
    public class Route :
        ViewPanelObjectWithCodeBase
    {
        private General.Address _AddressFrom;
        public General.Address AddressFrom { get { return _AddressFrom; } set { Set(ref _AddressFrom, value); } }

        private General.Address _AddressTo;
        public General.Address AddressTo { get { return _AddressTo; } set { Set(ref _AddressTo, value); } }

        private long _Distance;
        public long Distance { get { return _Distance; } set { Set(ref _Distance, value); } }

        private bool _DistanceIsInBothDirections;
        public bool DistanceIsInBothDirections { get { return _DistanceIsInBothDirections; } set { Set(ref _DistanceIsInBothDirections, value); } }
    }

    public class RouteConverter :
        ViewObjectConverter<Route, BL.Entities.Route>
    {
        private readonly Func<IGeneralObjects<View.General.Address>> _addresses;

        public RouteConverter(
            Func<IGeneralObjects<View.General.Address>> addresses
            )
        {
            _addresses = addresses;
        }

        protected override void OnInitialize(IMappingExpression<BL.Entities.Route, Route> mapingExpr)
        {
            mapingExpr
                .ForMember(
                    r => r.AddressFrom,
                    cfg => cfg.MapFrom(
                        r => _addresses.ConvertByGet(r.AddressFrom)
                        ))
                .ForMember(
                    r => r.AddressTo,
                    cfg => cfg.MapFrom(
                        r => _addresses.ConvertByGet(r.AddressTo)
                        ))
            ;
        }
    }
}
