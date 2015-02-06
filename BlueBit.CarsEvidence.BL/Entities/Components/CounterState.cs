using BlueBit.CarsEvidence.BL.Repositories;
using System;

namespace BlueBit.CarsEvidence.BL.Entities.Components
{
    public class CounterState :
        ComponentBase
    {
        public DateTime Date { get; set; }
        public long Counter { get; set; }
    }
}
