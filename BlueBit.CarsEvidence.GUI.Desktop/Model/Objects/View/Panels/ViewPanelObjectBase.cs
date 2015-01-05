using BlueBit.CarsEvidence.BL.Alghoritms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public abstract class ViewPanelObjectWithCodeBase :
        ViewPanelObjectBase,
        IObjectWithGetCode
    {
        private string _code;
        public string Code { get { return _code; } set { Set(ref _code, value); } }

        public override sealed string DescriptionForToolTip { get { return this.GetDescriptionForToolTip(); } }

        static ViewPanelObjectWithCodeBase()
        {
            RegisterPropertyDependency<ViewPanelObjectWithCodeBase>()
                .Add(x => x.DescriptionForToolTip, x => x.Code);
        }
    }
}
