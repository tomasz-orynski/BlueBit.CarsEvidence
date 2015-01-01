using System;

namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.View.Panels
{
    [Attributes.ShowInPanelView]
    [Attributes.EntityType(typeof(BL.Entities.Car))]
    [Attributes.ConverterType(typeof(ViewObjectConverter<,>))]
    public class Car : 
        ViewObjectWithCodeBase
    {
        private string _registerNumber;
        public string RegisterNumber { get { return _registerNumber; } set { Set(ref _registerNumber, value); } }

        private string _brandInfo;
        public string BrandInfo { get { return _brandInfo; } set { Set(ref _brandInfo, value); } }

        private DateTime _evidenceDateBegin;
        public DateTime EvidenceDateBegin { get { return _evidenceDateBegin; } set { Set(ref _evidenceDateBegin, value); } }

        private DateTime? _evidenceDateEnd;
        public DateTime? EvidenceDateEnd { get { return _evidenceDateEnd; } set { Set(ref _evidenceDateEnd, value); } }

        private long _evidenceCounterBegin;
        public long EvidenceCounterBegin { get { return _evidenceCounterBegin; } set { Set(ref _evidenceCounterBegin, value); } }

        private long? _evidenceCounterEnd;
        public long? EvidenceCounterEnd { get { return _evidenceCounterEnd; } set { Set(ref _evidenceCounterEnd, value); } }
    }
}
