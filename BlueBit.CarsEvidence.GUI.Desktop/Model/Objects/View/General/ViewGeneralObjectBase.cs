using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.View.General
{
    public interface IViewGeneralObject
    {
        string Description { get; }
    }

    public abstract class ViewGeneralObjectBase :
        ViewObjectBase,
        IViewGeneralObject
    {
        public abstract string Description { get; }
    }

    public abstract class ViewGeneralObjectWithCodeBase :
        ViewGeneralObjectBase
    {
        private string _code;
        public string Code { get { return _code; } set { Set(ref _code, value); } }

        public override sealed string Description
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append("»");

                if (string.IsNullOrEmpty(Code))
                    sb.AppendFormat("ID={0}", ID);
                else
                    sb.Append(Code);

                sb.Append("«");
                return sb.ToString();
            }
        }

        static ViewGeneralObjectWithCodeBase()
        {
            RegisterPropertyDependency<ViewGeneralObjectWithCodeBase>()
                .Add(x => x.Description, x => x.Code);
        }
    }
}
