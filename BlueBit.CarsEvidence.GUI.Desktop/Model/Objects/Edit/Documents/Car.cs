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
        EditDocumentObjectWithCodeBase
    {
        private string _registerNumber;
        [Required]
        [MaxLength(BL.Configuration.Consts.LengthRegisterNumber)]
        [Key]
        public string RegisterNumber { get { return _registerNumber; } set { Set(ref _registerNumber, value); } }

        private string _brandInfo;
        [MaxLength(BL.Configuration.Consts.LengthText)]
        public string BrandInfo { get { return _brandInfo; } set { Set(ref _brandInfo, value); } }

        private CounterState _evidenceBegin;
        public CounterState EvidenceBegin { get { return _evidenceBegin; } set { Set(ref _evidenceBegin, value); } }

        public bool EvidenceBeginState {
            get { return _evidenceBegin != null; }
            set
            {
                if (value)
                {
                    EvidenceBegin = new CounterState() { Date = DateTime.Today };
                }
                else
                {
                    EvidenceBegin = null;
                    EvidenceEnd = null;
                }
            }
        }

        private CounterState _evidenceEnd;
        public CounterState EvidenceEnd { get { return _evidenceEnd; } set { Set(ref _evidenceEnd, value); } }

        public bool EvidenceEndState
        {
            get { return _evidenceBegin != null && _evidenceEnd != null; }
            set
            {
                EvidenceEnd = value
                    ? new CounterState() { Date = DateTime.Today }
                    : null;
            }
        }

        private string _Info;
        [MaxLength(BL.Configuration.Consts.LengthInfo)]
        public string Info { get { return _Info; } set { Set(ref _Info, value); } }

        static Car()
        {
            RegisterPropertyDependency<Car>()
                .Add(x => x.EvidenceBeginState, x => x.EvidenceBegin)
                .Add(x => x.EvidenceEndState, x => x.EvidenceBegin, x => x.EvidenceEnd);
        }
    }
}
