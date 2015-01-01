using System;

namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Attributes
{
    public class EntityTypeAttribute : Attribute
    {
        public Type EntityType { get; set; }

        public EntityTypeAttribute(Type entityType)
        {
            EntityType = entityType;
        }
    }
}
