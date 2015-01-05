using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueBit.CarsEvidence.BL.Alghoritms
{
    public interface IObjectWithGetID
    {
        long ID { get; }
    }

    public interface IObjectWithSetID
    {
        long ID { set; }
    }

    public interface IObjectWithGetCode
    {
        string Code { get; }
    }

    public static class IDExtensions
    {
        private static IEnumerable<long> Range()
        {
            var id = -1L;
            do {
                yield return id;
            } while (--id > long.MinValue);
        }

        public static long GetTempID<T>(this IEnumerable<T> @this)
            where T: IObjectWithGetID
        {
            Contract.Assert(@this != null);

            return Range()
                .Except(@this.Select(_ => _.ID < 0 ? _.ID : 0))
                .First();
        }
    }
}
