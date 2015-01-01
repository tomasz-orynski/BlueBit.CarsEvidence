
namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.View.General
{
    [Attributes.EntityType(typeof(BL.Entities.Person))]
    [Attributes.ConverterType(typeof(ViewObjectConverter<,>))]
    public class Person :
        ViewGeneralObjectWithCodeBase
    {
    }
}
