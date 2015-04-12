using BlueBit.CarsEvidence.BL.Repositories;
using System;

namespace BlueBit.CarsEvidence.BL.Entities.Components
{
    public class ValueState<T> :
        ComponentBase
        where T: struct
    {
        public DateTime Date { get; set; }
        public T Value { get; set; }
    }
}
