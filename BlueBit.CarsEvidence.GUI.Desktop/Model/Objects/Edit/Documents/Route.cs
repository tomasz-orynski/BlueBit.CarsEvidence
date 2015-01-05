using AutoMapper;
using BlueBit.CarsEvidence.BL.Repositories;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit.Documents
{
    [Attributes.EditInDocumentView]
    [Attributes.EntityType(typeof(BL.Entities.Route))]
    [Attributes.ConverterType(typeof(RouteConverter))]
    public class Route :
        EditDocumentObjectWithCodeBase
    {
        public ObservableCollection<View.General.Address> AllAddresses { get { return _addresses().Items; } }

        private View.General.Address _AddressFrom;
        [Required]
        public View.General.Address AddressFrom { get { return _AddressFrom; } set { Set(ref _AddressFrom, value); } }

        private View.General.Address _AddressTo;
        [Required]
        public View.General.Address AddressTo { get { return _AddressTo; } set { Set(ref _AddressTo, value); } }

        private long _Distance;
        [Required]
        public long Distance { get { return _Distance; } set { Set(ref _Distance, value); } }

        private bool _DistanceIsInBothDirections;
        [Required]
        public bool DistanceIsInBothDirections { get { return _DistanceIsInBothDirections; } set { Set(ref _DistanceIsInBothDirections, value); } }

        private readonly Func<IViewObjects<View.General.Address>> _addresses;

        public Route(
            Func<IViewObjects<View.General.Address>> addresses
            )
        {
            _addresses = addresses;
        }
    }

    public class RouteConverter :
        EditObjectConverter<Route, BL.Entities.Route>
    {
        private readonly Func<IDbRepositories> _repository;
        private readonly Func<IViewObjects<View.General.Address>> _addresses;

        public RouteConverter(
            Func<IDbRepositories> repository,
            Func<IViewObjects<View.General.Address>> addresses
            )
        {
            _repository = repository;
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

        protected override void OnInitialize(IMappingExpression<Route, BL.Entities.Route> mapingExpr)
        {
            mapingExpr
                .ForMember(
                    r => r.AddressFrom,
                    cfg => cfg.MapFrom(
                        r => _repository.ConvertByLoad<BL.Entities.Address>(r.AddressFrom.ID)
                        )
                )
                .ForMember(
                    r => r.AddressTo,
                    cfg => cfg.MapFrom(
                        r => _repository.ConvertByLoad<BL.Entities.Address>(r.AddressTo.ID)
                        )
                )
                ;
        }
    }
}
