
namespace BlueBit.CarsEvidence.Commons.Templates
{
    public interface ISingleton<out T> :
        ICreator<T>
    {
        T GetInstance();
    }

    public interface ICreator<out T>
    {
        T Create();
    }

    public interface ISingletonCreator<out T> :
        ICreator<T>,
        ISingleton<T>
    {
    }

    public interface ISingletonForItem<out T, in TItem>
        where T : IObjectWithItem<TItem>
    {
        T GetInstance(TItem item);
    }

    public interface ICreatorForItem<out T, in TItem>
        where T : IObjectWithItem<TItem>
    {
        T Create(TItem item);
    }

    public interface ISingletonCreatorForItem<out T, in TItem> :
        ICreatorForItem<T, TItem>,
        ISingletonForItem<T, TItem>
        where T : IObjectWithItem<TItem>
    {
    }
}
