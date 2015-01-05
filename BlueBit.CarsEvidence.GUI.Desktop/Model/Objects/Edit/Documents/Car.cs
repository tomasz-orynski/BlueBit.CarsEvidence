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

        private DateTime? _evidenceDateBegin /* = DateTime.Now*/;
        [Required]
        [DateRange]
        public DateTime? EvidenceDateBegin { get { return _evidenceDateBegin; } set { Set(ref _evidenceDateBegin, value); } }

        private DateTime? _evidenceDateEnd;
        [DateRange]
        public DateTime? EvidenceDateEnd { get { return _evidenceDateEnd; } set { Set(ref _evidenceDateEnd, value); } }

        private long _evidenceCounterBegin;
        public long EvidenceCounterBegin { get { return _evidenceCounterBegin; } set { Set(ref _evidenceCounterBegin, value); } }

        private long? _evidenceCounterEnd;
        public long? EvidenceCounterEnd { get { return _evidenceCounterEnd; } set { Set(ref _evidenceCounterEnd, value); } }
    }
}
