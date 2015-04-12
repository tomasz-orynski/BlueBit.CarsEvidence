using BlueBit.CarsEvidence.Commons.Templates;
using System;
using System.Diagnostics.Contracts;

namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.View.General
{
    public interface IViewGeneralObject :
        IViewObject,
        IObjectWithDescription,
        IObjectWithDescriptionForToolTip,
        IComparable
    {
    }

    public static class ViewGeneralObjectExtensions
    {
        public static int CompareDescriptionTo(this IViewGeneralObject @this, object obj)
        {
            Contract.Assert(@this != null);

            var objWithDesc = obj as IObjectWithDescription;
            if (objWithDesc == null) return 0;

            var desc = @this.Description;
            var objDesc = objWithDesc.Description;
            return string.Compare(desc, objDesc);
        }
    }

    public abstract class ViewGeneralObjectBase :
        ViewObjectBase,
        IViewGeneralObject
    {
        public abstract string Description { get; }
        public abstract string DescriptionForToolTip { get; }

        public int CompareTo(object obj) { return this.CompareDescriptionTo(obj); }
    }

    public abstract class ViewGeneralObjectWithCodeInfoBase :
        ViewGeneralObjectBase,
        IObjectWithGetCode,
        IObjectWithGetInfo
    {
        private string _code;
        public string Code { get { return _code; } set { _Set(ref _code, value); } }

        private string _info;
        public string Info { get { return _info; } set { _Set(ref _info, value); } }

        public override sealed string Description { get { return this.GetDescription(); } }
        public override sealed string DescriptionForToolTip { get { return this.GetDescriptionForToolTip(); } }

        static ViewGeneralObjectWithCodeInfoBase()
        {
            RegisterPropertyDependency<ViewGeneralObjectWithCodeInfoBase>()
                .Add(_ => _.Description, _ => _.Code)
                .Add(_ => _.DescriptionForToolTip, _ => _.Code, _ => _.Info);
        }
    }
}
