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

        private ValueState<long> _evidenceBeg;
        public ValueState<long> EvidenceBeg { get { return _evidenceBeg; } set { _Set(ref _evidenceBeg, value); } }

        private ValueState<long> _evidenceEnd;
        public ValueState<long> EvidenceEnd { get { return _evidenceEnd; } set { _Set(ref _evidenceEnd, value); } }
    }
}
