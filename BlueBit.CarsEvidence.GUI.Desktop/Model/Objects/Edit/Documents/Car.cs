using BlueBit.CarsEvidence.BL.Entities.Components;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Attributes.Validation;
using System;
using System.ComponentModel.DataAnnotations;

namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit.Documents
{
    [Attributes.EditInDocumentView]
    [Attributes.EntityType(typeof(BL.Entities.Car))]
    [Attributes.ConverterType(typeof(EditObjectConverter<,>))]
    public class Car : 
        EditDocumentObjectWithCodeInfoBase
    {
        private string _registerNumber;
        [Required]
        [MaxLength(BL.Configuration.Consts.LengthRegisterNumber)]
        [Key]
        public string RegisterNumber { get { return _registerNumber; } set { _Set(ref _registerNumber, value); } }

        private string _brandInfo;
        [MaxLength(BL.Configuration.Consts.LengthText)]
        public string BrandInfo { get { return _brandInfo; } set { _Set(ref _brandInfo, value); } }

        private ValueState<long> _evidenceBegin;
        public ValueState<long> EvidenceBegin { get { return _evidenceBegin; } set { _Set(ref _evidenceBegin, value); } }

        public bool EvidenceBeginState {
            get { return _evidenceBegin != null; }
            set
            {
                if (value)
                {
                    EvidenceBegin = new ValueState<long>() { Date = DateTime.Today };
                }
                else
                {
                    EvidenceBegin = null;
                    EvidenceEnd = null;
                }
            }
        }

        private ValueState<long> _evidenceEnd;
        public ValueState<long> EvidenceEnd { get { return _evidenceEnd; } set { _Set(ref _evidenceEnd, value); } }

        public bool EvidenceEndState
        {
            get { return _evidenceBegin != null && _evidenceEnd != null; }
            set
            {
                EvidenceEnd = value
                    ? new ValueState<long>() { Date = DateTime.Today }
                    : null;
            }
        }

        static Car()
        {
            RegisterPropertyDependency<Car>()
                .Add(x => x.EvidenceBeginState, x => x.EvidenceBegin)
                .Add(x => x.EvidenceEndState, x => x.EvidenceBegin, x => x.EvidenceEnd);
        }
    }
}
