
namespace BlueBit.CarsEvidence.Commons.Templates
{
    public interface IObjectWithGetID
    {
        long ID { get; }
    }

    public interface IObjectWithSetID
    {
        long ID { set; }
    }

    public interface IObjectWithGetCode
    {
        string Code { get; }
    }

    public interface IObjectWithGetInfo
    {
        string Info { get; }
    }
}
