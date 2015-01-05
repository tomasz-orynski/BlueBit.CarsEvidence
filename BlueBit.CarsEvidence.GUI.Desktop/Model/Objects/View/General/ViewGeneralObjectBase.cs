using BlueBit.CarsEvidence.BL.Alghoritms;
using System.Text;

namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.View.General
{
    public interface IViewGeneralObject :
        IViewObject,
        IObjectWithDescription
    {
    }

    public abstract class ViewGeneralObjectBase :
        ViewObjectBase,
        IViewGeneralObject
    {
        public abstract string Description { get; }
    }

    public abstract class ViewGeneralObjectWithCodeBase :
        ViewGeneralObjectBase,
        IObjectWithGetCode
    {
        private string _code;
        public string Code { get { return _code; } set { Set(ref _code, value); } }

        public override sealed string Description { get { return this.GetDescription(); } }

        static ViewGeneralObjectWithCodeBase()
        {
            RegisterPropertyDependency<ViewGeneralObjectWithCodeBase>()
                .Add(x => x.Description, x => x.Code);
        }
    }
}
