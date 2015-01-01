using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueBit.CarsEvidence.Commons.Templates
{
    public interface IObjectWithItem<TItem>
    {
        TItem Item { get; }
    }
}
