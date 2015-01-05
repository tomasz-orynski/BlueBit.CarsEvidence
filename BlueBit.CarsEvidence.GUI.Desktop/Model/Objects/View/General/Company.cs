
namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.View.General
{
    [Attributes.EntityType(typeof(BL.Entities.Company))]
    [Attributes.ConverterType(typeof(ViewObjectConverter<,>))]
    public class Company : 
        ViewGeneralObjectWithCodeBase,
        IObjectWithDescriptionForTitle
    {
        public string DescriptionForTitle { get { return this.GetDescriptionForTitle(); } }

        static Company()
        {
            RegisterPropertyDependency<Company>()
                .Add(x => x.DescriptionForTitle, x => x.Code);
        }
    }
}
