using AutoMapper;
using BlueBit.CarsEvidence.BL.Configuration;
using BlueBit.CarsEvidence.BL.Repositories;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit.Documents
{
    [Attributes.EditInDocumentView]
    [Attributes.CanEditAllInDocumentView]
    [Attributes.EntityType(typeof(BL.Entities.Company))]
    [Attributes.ConverterType(typeof(CompanyConverter))]
    public class Company :
        EditDocumentObjectWithCodeBase
    {
        public ObservableCollection<View.General.Address> AllAddresses { get { return _addresses().Items; } }

        private string _Name;
        [Required]
        [MaxLength(Consts.LengthText)]
        public virtual string Name { get { return _Name; } set { Set(ref _Name, value); } }

        private string _IdentifierNIP;
        [MaxLength(Consts.LengthIdentifierNIP)]
        public virtual string IdentifierNIP { get { return _IdentifierNIP; } set { Set(ref _IdentifierNIP, value); } }

        private string _IdentifierREGON;
        [MaxLength(Consts.LengthIdentifierREGON)]
        public virtual string IdentifierREGON { get { return _IdentifierREGON; } set { Set(ref _IdentifierREGON, value); } }

        private View.General.Address _Address;
        public View.General.Address Address { get { return _Address; } set { Set(ref _Address, value); } }

        private readonly Func<IViewObjects<View.General.Address>> _addresses;

        public Company(
            Func<IViewObjects<View.General.Address>> addresses
            )
        {
            _addresses = addresses;
        }
    }

    public class CompanyConverter :
        EditObjectConverter<Company, BL.Entities.Company>
    {
        private readonly Func<IDbRepositories> _repository;
        private readonly Func<IViewObjects<View.General.Address>> _addresses;

        public CompanyConverter(
            Func<IDbRepositories> repository,
            Func<IViewObjects<View.General.Address>> addresses
            )
        {
            _repository = repository;
            _addresses = addresses;
        }

        protected override void OnInitialize(IMappingExpression<BL.Entities.Company, Company> mapingExpr)
        {
            mapingExpr
                .ForMember(
                    r => r.Address,
                    cfg => cfg.MapFrom(
                        r => _addresses.ConvertByGet(r.Address)
                        ))
            ;
        }

        protected override void OnInitialize(IMappingExpression<Company, BL.Entities.Company> mapingExpr)
        {
            mapingExpr
                .ForMember(
                    r => r.Address,
                    cfg => cfg.MapFrom(
                        r => _repository.ConvertByLoad<BL.Entities.Address>(r.Address.ID)
                        )
                )
                ;
        }
    }
}
