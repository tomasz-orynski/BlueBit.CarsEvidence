
using System.ComponentModel.DataAnnotations;
namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit.Documents
{
    [Attributes.EditInDocumentView]
    [Attributes.EntityType(typeof(BL.Entities.Address))]
    [Attributes.ConverterType(typeof(EditObjectConverter<,>))]
    public class Address : 
        EditObjectWithCodeBase
    {
        private string _PostalCode;
        [Required]
        [MaxLength(BL.Configuration.Consts.LengthPostalCode)]
        [RegularExpression(BL.Configuration.Consts.MaskPostalCode)]
        public string PostalCode { get { return _PostalCode; } set { Set(ref _PostalCode, value); } }

        private string _City;
        [Required]
        [MaxLength(BL.Configuration.Consts.LengthText)]
        public string City { get { return _City; } set { Set(ref _City, value); } }

        private string _Street;
        [Required]
        [MaxLength(BL.Configuration.Consts.LengthText)]
        public string Street { get { return _Street; } set { Set(ref _Street, value); } }

        private string _BuildingNo;
        [Required]
        [MaxLength(BL.Configuration.Consts.LengthCode)]
        public string BuildingNo { get { return _BuildingNo; } set { Set(ref _BuildingNo, value); } }

        private string _LocalNo;
        [MaxLength(BL.Configuration.Consts.LengthCode)]
        public string LocalNo { get { return _LocalNo; } set { Set(ref _LocalNo, value); } }
    }
}
