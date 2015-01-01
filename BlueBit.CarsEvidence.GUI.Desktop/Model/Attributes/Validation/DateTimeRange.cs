using System;
using System.ComponentModel.DataAnnotations;

namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Attributes.Validation
{
    public class DateTimeRangeAttribute : RangeAttribute
    {
        public DateTimeRangeAttribute()
            : base(typeof(DateTime), BL.Configuration.Consts.ValueMinDateTime.ToString(), BL.Configuration.Consts.ValueMaxDateTime.ToString())
        {
        }
    }
}
