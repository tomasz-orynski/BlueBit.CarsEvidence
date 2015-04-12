
namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.View.Panels
{
    [Attributes.ShowInPanelView]
    [Attributes.EntityType(typeof(BL.Entities.Person))]
    [Attributes.ConverterType(typeof(ViewObjectConverter<,>))]
    public class Person :
        ViewPanelObjectWithCodeInfoBase
    {
        private string _FirstName;
        public string FirstName { get { return _FirstName; } set { _Set(ref _FirstName, value); } }

        private string _LastName;
        public string LastName { get { return _LastName; } set { _Set(ref _LastName, value); } }
    }
}

