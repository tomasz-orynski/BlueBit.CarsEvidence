using BlueBit.CarsEvidence.Commons.Templates;

namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.View.Panels
{
    public interface IViewPanelObject :
        IViewObject,
        IObjectWithDescriptionForToolTip
    {
    }

    public abstract class ViewPanelObjectBase :
        ViewObjectBase,
        IViewPanelObject
    {
        public abstract string DescriptionForToolTip { get; }
    }

    public abstract class ViewPanelObjectWithCodeInfoBase :
        ViewPanelObjectBase,
        IObjectWithGetCode,
        IObjectWithGetInfo
    {
        private string _code;
        public string Code { get { return _code; } set { _Set(ref _code, value); } }

        private string _info;
        public string Info { get { return _info; } set { _Set(ref _info, value); } }

        public override sealed string DescriptionForToolTip { get { return this.GetDescriptionForToolTip(); } }

        static ViewPanelObjectWithCodeInfoBase()
        {
            RegisterPropertyDependency<ViewPanelObjectWithCodeInfoBase>()
                .Add(_ => _.DescriptionForToolTip, _ => _.Code, _ => _.Info);
        }
    }
}
