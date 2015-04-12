using System.ComponentModel.DataAnnotations;

namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit.Documents
{
    [Attributes.EditInDocumentView]
    [Attributes.EntityType(typeof(BL.Entities.Person))]
    [Attributes.ConverterType(typeof(EditObjectConverter<,>))]
    public class Person :
        EditDocumentObjectWithCodeInfoBase
    {
        private string _FirstName;
        [Required]
        [MaxLength(BL.Configuration.Consts.LengthText)]
        public string FirstName { get { return _FirstName; } set { _Set(ref _FirstName, value); } }

        private string _LastName;
        [Required]
        [MaxLength(BL.Configuration.Consts.LengthText)]
        public string LastName { get { return _LastName; } set { _Set(ref _LastName, value); } }
    }
}
