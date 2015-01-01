using System.ComponentModel.DataAnnotations;

namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Attributes.Validation
{
    public class YearRangeAttribute : RangeAttribute
    {
        public YearRangeAttribute()
            : base(BL.Configuration.Consts.ValueMinYear, BL.Configuration.Consts.ValueMaxYear)
        {
        }
    }
}
