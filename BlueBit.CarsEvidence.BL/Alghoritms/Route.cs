using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace BlueBit.CarsEvidence.BL.Alghoritms
{
    public interface IRoute
    {
        long Distance { get; }
        bool DistanceIsInBothDirections { get; }
    }

    public static class RouteExt
    {
        public static long GetDistanceTotal(this IEnumerable<IRoute> @this)
        {
            Contract.Assert(@this != null);
            return @this
                .Where(_ => _ != null)
                .Sum(_ => _.Distance);
        }
    }
}
