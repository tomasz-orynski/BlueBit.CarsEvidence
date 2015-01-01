using System;

namespace BlueBit.CarsEvidence.GUI.Desktop.Configuration.Attributes
{
    public class RegisterAttribute : Attribute
    {
        public Type AsType { get; set; }
        public bool AsSingleton { get; set; }

        public RegisterAttribute(Type asType, bool asSingleton = false)
        {
            AsType = asType;
            AsSingleton = asSingleton;
        }
    }
}
