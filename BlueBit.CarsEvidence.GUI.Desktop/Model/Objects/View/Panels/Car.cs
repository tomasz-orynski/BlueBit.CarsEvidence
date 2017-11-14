using BlueBit.CarsEvidence.BL.Entities.Components;
using System;

namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.View.Panels
{
    [Attributes.ShowInPanelView]
    [Attributes.EntityType(typeof(BL.Entities.Car))]
    [Attributes.ConverterType(typeof(ViewObjectConverter<,>))]
    public class Car :
        ViewPanelObjectWithCodeInfoBase
    {
        private string _registerNumber;
        public string RegisterNumber { get { return _registerNumber; } set { _Set(ref _registerNumber, value); } }

        private string _brandInfo;
        public string BrandInfo { get { return _brandInfo; } set { _Set(ref _brandInfo, value); } }

        private ValueState<long> _evidenceBegin;
        public ValueState<long> EvidenceBegin { get { return _evidenceBegin; } set { _Set(ref _evidenceBegin, value); } }

        private ValueState<long> _evidenceEnd;
        public ValueState<long> EvidenceEnd { get { return _evidenceEnd; } set { _Set(ref _evidenceEnd, value); } }
    }
}
