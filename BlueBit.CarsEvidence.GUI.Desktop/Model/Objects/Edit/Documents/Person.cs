using System.ComponentModel.DataAnnotations;

namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit.Documents
{
    [Attributes.EditInDocumentView]
    [Attributes.EntityType(typeof(BL.Entities.Person))]
    [Attributes.ConverterType(typeof(EditObjectConverter<,>))]
    public class Person :
        EditDocumentObjectWithCodeBase
    {
        private string _FirstName;
        [Required]
        public string FirstName { get { return _FirstName; } set { Set(ref _FirstName, value); } }

        private string _LastName;
        [Required]
        public string LastName { get { return _LastName; } set { Set(ref _LastName, value); } }
    }
}
