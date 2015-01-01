using System;

namespace BlueBit.CarsEvidence.GUI.Desktop.Configuration.Attributes
{
    public class RegisterAllAttribute : Attribute
    {
        public bool AsSingleton { get; set; }

        public RegisterAllAttribute(bool asSingleton = false)
        {
            AsSingleton = asSingleton;
        }
    }
}
