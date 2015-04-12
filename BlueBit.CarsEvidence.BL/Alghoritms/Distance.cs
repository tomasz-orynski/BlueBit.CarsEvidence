using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace BlueBit.CarsEvidence.BL.Alghoritms
{
    public static class PeriodEntryDistanceExt
    {
        public static long GetDistance(this Entities.PeriodRouteEntry @this)
        {
            Contract.Assert(@this != null);
            return @this.Distance ?? @this.Route.Distance;
        }

        public static long GetDistanceTotal(this IEnumerable<Entities.PeriodRouteEntry> @this)
        {
            Contract.Assert(@this != null);
            return @this
                .Sum(_ => _.GetDistance());
        }
    }
}
