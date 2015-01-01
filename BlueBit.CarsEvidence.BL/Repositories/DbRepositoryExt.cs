using BlueBit.CarsEvidence.BL.Entities;
using BlueBit.CarsEvidence.BL.Alghoritms;
using System.Linq;
using System.Diagnostics.Contracts;

namespace BlueBit.CarsEvidence.BL.Repositories
{
    public static class DbRepositoryExt
    {
        public static long GetTotalDistanceForPrevMonths(
            this IDbRepositories @this, 
            long carID,
            int year,
            byte month)
        {
            Contract.Assert(@this != null);

            var car = @this.CreateQuery<Car>()
                .Where(_ => _.ID == carID)
                .SingleOrDefault();

            BL.Entities.Period periodAlias = null;
            var prevEntries = @this.CreateQuery<BL.Entities.PeriodEntry>()
                .JoinAlias(_ => _.Period, () => periodAlias)
                .Where(_ => periodAlias.Car.ID == carID)
                .Where(_ => periodAlias.Year < year || (periodAlias.Year == year && periodAlias.Month < month))
                .Fetch(_ => _.Route).Eager
                .List();
            var prevDistance = prevEntries.Select(_ => _.Route).GetDistanceTotal();
            return prevDistance + car.EvidenceCounterBegin;
        }

    }
}
