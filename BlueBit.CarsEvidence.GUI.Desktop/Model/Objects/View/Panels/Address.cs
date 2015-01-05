using System.Text;

namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.View.Panels
{
    [Attributes.ShowInPanelView]
    [Attributes.EntityType(typeof(BL.Entities.Address))]
    [Attributes.ConverterType(typeof(ViewObjectConverter<,>))]
    public class Address :
        ViewPanelObjectWithCodeBase
    {
        private string _PostalCode;
        public string PostalCode { get { return _PostalCode; } set { Set(ref _PostalCode, value); } }

        private string _City;
        public string City { get { return _City; } set { Set(ref _City, value); } }

        private string _Street;
        public string Street { get { return _Street; } set { Set(ref _Street, value); } }

        private string _BuildingNo;
        public string BuildingNo { get { return _BuildingNo; } set { Set(ref _BuildingNo, value); } }

        private string _LocalNo;
        public string LocalNo { get { return _LocalNo; } set { Set(ref _LocalNo, value); } }

        public string StreetWithNo { 
            get {
                var sb = new StringBuilder();
                if (string.IsNullOrEmpty(_Street)) return sb.ToString();
                sb.Append(_Street);
                if (string.IsNullOrEmpty(_BuildingNo)) return sb.ToString();
                sb.Append(" ");
                sb.Append(_BuildingNo);
                if (string.IsNullOrEmpty(_LocalNo)) return sb.ToString();
                sb.Append("/");
                sb.Append(_LocalNo);
                return sb.ToString();
            } 
        }

        static Address()
        {
            RegisterPropertyDependency<Address>()
                .Add(x => x.StreetWithNo, x => x.Street, x => x.BuildingNo, x => x.LocalNo);
        }
    }
}
