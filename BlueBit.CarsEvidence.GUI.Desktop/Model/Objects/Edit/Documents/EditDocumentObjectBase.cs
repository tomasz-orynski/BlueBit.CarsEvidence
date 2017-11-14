using BlueBit.CarsEvidence.Commons.Templates;
using System.ComponentModel.DataAnnotations;

namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit.Documents
{
    public interface IEditDocumentObject :
        IEditObject,
        IObjectWithDescription
    {
    }

    public abstract class EditDocumentObjectBase :
        EditObjectBase,
        IEditDocumentObject
    {
        public abstract string Description { get; }
    }

    public abstract class EditDocumentObjectWithCodeInfoBase :
        EditDocumentObjectBase,
        IEditObjectWithCode,
        IEditObjectWithInfo
    {
        private string _code;
        [Required]
        [MaxLength(BL.Configuration.Consts.LengthCode)]
        [Key]
        public string Code { get { return _code; } set { _Set(ref _code, value); } }

        private string _Info;
        [MaxLength(BL.Configuration.Consts.LengthInfo)]
        public string Info { get { return _Info; } set { _Set(ref _Info, value); } }

        public bool HasInfo { get { return !string.IsNullOrWhiteSpace(_Info); } }

        public override sealed string Description { get { return this.GetDescription(); } }

        static EditDocumentObjectWithCodeInfoBase()
        {
            RegisterPropertyDependency<EditDocumentObjectWithCodeInfoBase>()
                .Add(x => x.HasInfo, x => x.Info);
        }
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

    public abstract class EditDocumentObjectChildWithInfoBase :
        EditDocumentObjectChildBase,
        IEditObjectWithInfo
    {
        private string _Info;
        [MaxLength(BL.Configuration.Consts.LengthInfo)]
        public string Info { get { return _Info; } set { _Set(ref _Info, value); } }

        public bool HasInfo { get { return !string.IsNullOrWhiteSpace(_Info); } }

        static EditDocumentObjectChildWithInfoBase()
        {
            RegisterPropertyDependency<EditDocumentObjectChildWithInfoBase>()
                .Add(x => x.HasInfo, x => x.Info);
        }
    }
}
