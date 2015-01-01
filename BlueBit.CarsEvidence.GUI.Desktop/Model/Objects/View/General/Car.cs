using System.Text;

namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.View.General
{
    [Attributes.EntityType(typeof(BL.Entities.Car))]
    [Attributes.ConverterType(typeof(ViewObjectConverter<,>))]
    public class Car :
        ViewGeneralObjectWithCodeBase
    {
    }
}
