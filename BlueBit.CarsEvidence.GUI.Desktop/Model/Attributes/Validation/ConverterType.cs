using System;

namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Attributes
{
    public class ConverterTypeAttribute : Attribute
    {
        public Type ConverterType { get; set; }

        public ConverterTypeAttribute(Type converterType)
        {
            ConverterType = converterType;
        }
    }
}
