using System;

namespace BlueBit.CarsEvidence.GUI.Desktop.Resources
{
    public interface IInfo<out T>
    {
        T Key { get; }
        string Name { get; }
    }
}
