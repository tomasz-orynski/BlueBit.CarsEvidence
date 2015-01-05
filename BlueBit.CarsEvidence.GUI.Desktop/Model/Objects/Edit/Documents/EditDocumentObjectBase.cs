using BlueBit.CarsEvidence.BL.Alghoritms;
using System.ComponentModel.DataAnnotations;

namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit.Documents
{
    public interface IEditDocumentObject :
        IEditObject,
        IObjectWithDescriptionForTitle
    {
    }

    public abstract class EditDocumentObjectBase :
        EditObjectBase,
        IEditDocumentObject
    {
        public abstract string DescriptionForTitle { get; }
    }

    public abstract class EditDocumentObjectWithCodeBase :
        EditDocumentObjectBase,
        IObjectWithGetCode
    {
        private string _code;
        [Required]
        [MaxLength(BL.Configuration.Consts.LengthCode)]
        [Key]
        public string Code { get { return _code; } set { Set(ref _code, value); } }

        public override sealed string DescriptionForTitle { get { return this.GetDescriptionForTitle(); } }
    }

    public interface IEditDocumentObjectChild :
        IEditObject
    {
    }

    public abstract class EditDocumentObjectChildBase :
        EditObjectBase,
        IEditDocumentObjectChild
    {
    }
}
