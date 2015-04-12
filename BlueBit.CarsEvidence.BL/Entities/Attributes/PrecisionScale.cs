using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueBit.CarsEvidence.BL.Entities.Attributes
{
    public class PrecisionScaleAttribute :
        Attribute
    {
        public int Precision { get; set; }
        public int Scale { get; set; }

        public PrecisionScaleAttribute(int precision, int scale = 0)
        {
            Precision = precision;
            Scale = scale;
        }
    }
}
