using System;

namespace BlueBit.CarsEvidence.BL.Alghoritms.Validation
{
    public static class DateValidation
    {
        public static bool IsYearValid(this int @this)
        {
            return @this >= Configuration.Consts.ValueMinYear
                && @this <= Configuration.Consts.ValueMaxYear;
        }
        
        public static bool IsMonthValid(this byte @this)
        {
            return @this >= Configuration.Consts.ValueMinMonth
                && @this <= Configuration.Consts.ValueMaxMonth;
        }

        public static bool IsDateValid(this DateTime @this)
        {
            return @this >= Configuration.Consts.ValueMinDate
                && @this <= Configuration.Consts.ValueMaxDate
                && @this == @this.Date;
        }

        public static bool IsDateTimeValid(this DateTime @this)
        {
            return @this >= Configuration.Consts.ValueMinDateTime
                && @this <= Configuration.Consts.ValueMaxDateTime;
        }
    }
}
