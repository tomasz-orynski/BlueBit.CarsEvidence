using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueBit.CarsEvidence.Commons.Templates
{
    public interface IObjectForType
    {
        Type ForType { get; }
    }

    public interface IObjectForType<out T>
        where T: struct, IConvertible //enum
    {
        T ForType { get; }
    }
}
