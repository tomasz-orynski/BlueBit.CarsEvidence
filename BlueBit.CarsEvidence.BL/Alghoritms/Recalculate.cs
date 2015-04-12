using AutoMapper;
using BlueBit.CarsEvidence.BL.Entities.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace BlueBit.CarsEvidence.BL.Alghoritms
{
    internal static class RecalculateExtensions
    {
        public static IEnumerable<Entities.Period> RecalculateStats(this IEnumerable<Entities.Period> @this)
        {
            Contract.Assert(@this != null);
            Contract.ForAll(@this, _ => _ != null);

            var groups = @this.GroupBy(_ => _.Car);
            groups.Each(periodsGroup =>
            {
                var carDistance = periodsGroup.Key.EvidenceBeg.Value;
                var carFuelVolume = 0m;
                var carFuelAmount = 0m;

                var items = periodsGroup.OrderBy(_ => Tuple.Create(_.Year, _.Month));
                items.Each(period =>
                {
                    Contract.ForAll(period.RouteEntries, _ => _ != null);
                    Contract.ForAll(period.FuelEntries, _ => _ != null && _.Purchase != null);

                    {
                        var stats = ValueStatsExt.CreateFrom(period.RouteEntries.Select(PeriodEntryDistanceExt.GetDistance), carDistance);
                        period.RouteStats = stats;
                        carDistance = stats.ValueEnd;
                    }
                    {
                        var stats = PurchaseStatsExt.CreateFrom(period.FuelEntries.Select(_ => _.Purchase), _ => _.Volume, _ => _.Amount, carFuelVolume, carFuelAmount);
                        period.FuelStats = stats;
                        carFuelVolume = stats.VolumeEnd;
                        carFuelAmount = stats.AmountEnd;
                    }
                });
            });
            return @this;
        }
    }

}
