using System;

namespace BlueBit.CarsEvidence.BL.Configuration
{
    public static class Consts
    {
        public const int LengthCode = 30;
        public const int LengthText = 250;
        public const int LengthInfo = 4000;
        public const int LengthRegisterNumber = 8;
        public const int LengthPostalCode = 6;
        public const int LengthIdentifierNIP = 13;
        public const int LengthIdentifierREGON = 14;

        public const string FormatDate = "YYYY-mm-dd";

        public static readonly int ValueMinYear = 2000;
        public static readonly int ValueMaxYear = 9999;

        public static readonly int ValueMinMonth = 1;
        public static readonly int ValueMaxMonth = 12;

        public static readonly DateTime ValueMinDate = new DateTime(ValueMinYear, ValueMinMonth, 1, 0, 0, 0);
        public static readonly DateTime ValueMaxDate = new DateTime(ValueMaxYear, ValueMaxMonth, 31, 0, 0, 0);

        public static readonly DateTime ValueMinDateTime = new DateTime(ValueMinYear, ValueMinMonth, 1, 0, 0, 0);
        public static readonly DateTime ValueMaxDateTime = new DateTime(ValueMaxYear, ValueMaxMonth, 31, 23, 59, 59);

        public const string MaskPostalCode = "^[0-9]{2}-[0-9]{3}$";

        public const int AmountSPrecision = 9;
        public const int Amount1Precision = 6;
        public const int AmountScale = 2;
        public const int VolumeSPrecision = 7;
        public const int Volume1Precision = 4;
        public const int VolumeScale = 1;
    }
}
