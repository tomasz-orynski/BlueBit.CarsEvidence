using BlueBit.CarsEvidence.BL.Alghoritms;
using System;
using System.Diagnostics.Contracts;
using System.Text;

namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.View.General
{
    public interface IViewGeneralObject :
        IViewObject,
        IObjectWithDescription,
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

        public int CompareTo(object obj) { return this.CompareDescriptionTo(obj); }
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
