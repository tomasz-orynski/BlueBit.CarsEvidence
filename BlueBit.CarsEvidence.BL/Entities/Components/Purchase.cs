using BlueBit.CarsEvidence.BL.Entities.Attributes;
using BlueBit.CarsEvidence.BL.Entities.Enums;
using BlueBit.CarsEvidence.BL.Repositories;

namespace BlueBit.CarsEvidence.BL.Entities.Components
{
    public class Purchase<TType> :
        ComponentBase
    {
        public TType Type { get; set; }
        [PrecisionScale(Configuration.Consts.Volume1Precision, Configuration.Consts.VolumeScale)]
        public decimal Volume { get; set; }
        [PrecisionScale(Configuration.Consts.Amount1Precision, Configuration.Consts.AmountScale)]
        public decimal Amount { get; set; }
    }

    public class FuelPurchase :
        Purchase<FuelType>
    {
    }
}
