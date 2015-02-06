using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace BlueBit.CarsEvidence.BL.Alghoritms
{
    public static class PeriodEntryDistanceExt
    {
        public static long GetDistance(this Entities.PeriodEntry @this)
        {
            Contract.Assert(@this != null);
            return @this.Distance ?? @this.Route.Distance;
        }

        public static long GetDistanceTotal(this IEnumerable<Entities.PeriodEntry> @this)
        {
            Contract.Assert(@this != null);
            return @this
                .Sum(_ => _.GetDistance());
        }
    }

    public static class PeriodDistanceExt
    {
        public static long GetDistanceTotal(this IEnumerable<Entities.Period> @this)
        {
            Contract.Assert(@this != null);
            return @this
                .Sum(_ => _.DistanceTotal);
        }
    }

    public static class RouteDistanceExt
    {
        public static long GetDistanceTotal(this IEnumerable<Entities.Route> @this)
        {
            Contract.Assert(@this != null);
            return @this
                .Sum(_ => _.Distance);
        }

    }
}
