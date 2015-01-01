using System;
using System.ComponentModel.DataAnnotations;

namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Attributes.Validation
{
    public class DateRangeAttribute : RangeAttribute
    {
        public DateRangeAttribute()
            : base(typeof(DateTime), BL.Configuration.Consts.ValueMinDate.ToString(), BL.Configuration.Consts.ValueMaxDate.ToString())
        {
        }

        public override bool IsValid(object value)
        {
            if (base.IsValid(value))
            {
                if (value == null) return true;
                var dt = (DateTime)value;
                return dt == dt.Date;
            }
            return false;
        }
    }
}
