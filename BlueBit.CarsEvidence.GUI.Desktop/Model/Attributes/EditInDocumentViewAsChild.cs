using System;

namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Attributes
{
    public class EditInDocumentViewAsChildAttribute : Attribute
    {
        public Type ParentType { get; set; }

        public EditInDocumentViewAsChildAttribute(Type parentType)
        {
            ParentType = parentType;
        }
    }
}
