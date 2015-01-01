using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueBit.CarsEvidence.Commons.Templates
{
    public interface ICreator<out T>
    {
        T Create();
    }

    public interface ISingletonCreator<out T> :
        ICreator<T>
    {
        T GetInstance();
    }

    public interface ICreatorForItem<out T, in TItem>
        where T : IObjectWithItem<TItem>
    {
        T Create(TItem item);
    }

    public interface ISingletonCreatorForItem<out T, in TItem> :
        ICreatorForItem<T, TItem>
        where T : IObjectWithItem<TItem>
    {
        T GetInstance(TItem item);
    }
}
