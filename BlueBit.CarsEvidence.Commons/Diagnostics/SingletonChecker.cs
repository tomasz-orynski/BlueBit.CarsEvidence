using System.Diagnostics.Contracts;

namespace BlueBit.CarsEvidence.Commons.Diagnostics
{
    public class SingletonChecker<T>
    {
        private static long counter = 0;
        public SingletonChecker()
        {
            Contract.Assert(counter == 0);
            ++counter;
        }
    }
}
