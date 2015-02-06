using AutoMapper;
using BlueBit.CarsEvidence.BL.Alghoritms;

namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.View.General
{
    [Attributes.EntityType(typeof(BL.Entities.Route))]
    [Attributes.ConverterType(typeof(ViewObjectConverter<,>))]
    public class Route :
        ViewGeneralObjectWithCodeBase
    {
        private long _Distance;
        public long Distance { get { return _Distance; } set { Set(ref _Distance, value); } }

        private bool _DistanceIsInBothDirections;
        public bool DistanceIsInBothDirections { get { return _DistanceIsInBothDirections; } set { Set(ref _DistanceIsInBothDirections, value); } }
    }
}
