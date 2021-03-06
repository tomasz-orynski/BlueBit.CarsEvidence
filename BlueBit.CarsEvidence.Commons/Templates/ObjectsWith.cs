﻿using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace BlueBit.CarsEvidence.Commons.Templates
{
    public static class ObjectsWithIDExtensions
    {
        private static IEnumerable<long> Range()
        {
            var id = -1L;
            do
            {
                yield return id;
            } while (--id > long.MinValue);
        }

        public static long GetTempID<T>(this IEnumerable<T> @this)
            where T : IObjectWithGetID
        {
            Contract.Assert(@this != null);

            return Range()
                .Except(@this.Select(_ => _.ID < 0 ? _.ID : 0))
                .First();
        }
    }
}
