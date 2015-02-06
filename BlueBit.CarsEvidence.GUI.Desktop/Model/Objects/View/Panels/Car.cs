using BlueBit.CarsEvidence.BL.Entities.Components;
using System;

namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.View.Panels
{
    [Attributes.ShowInPanelView]
    [Attributes.EntityType(typeof(BL.Entities.Car))]
    [Attributes.ConverterType(typeof(ViewObjectConverter<,>))]
    public class Car :
        ViewPanelObjectWithCodeBase
    {
        private string _registerNumber;
        public string RegisterNumber { get { return _registerNumber; } set { Set(ref _registerNumber, value); } }

        private string _brandInfo;
        public string BrandInfo { get { return _brandInfo; } set { Set(ref _brandInfo, value); } }

        private CounterState _evidenceBegin;
        public CounterState EvidenceBegin { get { return _evidenceBegin; } set { Set(ref _evidenceBegin, value); } }

        private CounterState _evidenceEnd;
        public CounterState EvidenceEnd { get { return _evidenceEnd; } set { Set(ref _evidenceEnd, value); } }
    }
}
