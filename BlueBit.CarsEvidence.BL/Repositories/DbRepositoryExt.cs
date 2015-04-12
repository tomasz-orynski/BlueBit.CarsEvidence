using BlueBit.CarsEvidence.BL.Entities;
using BlueBit.CarsEvidence.BL.Alghoritms;
using System.Linq;
using System.Diagnostics.Contracts;
using NHibernate.Criterion;
using BlueBit.CarsEvidence.Commons.Helpers;

namespace BlueBit.CarsEvidence.BL.Repositories
{
    public static class DbRepositoryExt
    {
        /*TODO
        public static long GetDistanceTotalForPrevMonths(
            this IDbRepositories @this, 
            long carID,
            int year,
            byte month)
        {
            Contract.Assert(@this != null);

            var car = @this.CreateQuery<Car>()
                .Where(_ => _.ID == carID)
                .SingleOrDefault();

            var prevEntriesSum = @this.CreateQuery<Entities.Period>()
                .Where(_ => _.Car.ID == carID)
                .Where(_ => _.Year < year || (_.Year == year && _.Month < month))
                .Select(Projections.Sum<Entities.Period>(_ => _.RouteEntries.GetDistanceTotal()))
                .SingleOrDefault<long>();
            return prevEntriesSum + car.EvidenceBeg.GetSafeValue(_ => _.Value);
        }
         */

        public static Entities.Period GetPreviousPeriod(
            this IDbRepositories @this,
            long carID,
            int year,
            byte month)
        {
            return @this.CreateQuery<Entities.Period>()
                .Where(_ => _.Car.ID == carID)
                .Where(_ => _.Year < year || (_.Year == year && _.Month < month))
                .OrderBy(_ => _.Year).Desc
                .OrderBy(_ => _.Month).Desc
                .Take(1)
                .SingleOrDefault();
        }
    }
}
