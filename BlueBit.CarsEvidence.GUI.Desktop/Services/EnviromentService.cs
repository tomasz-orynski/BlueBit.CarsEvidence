using BlueBit.CarsEvidence.GUI.Desktop.Configuration.Attributes;
using System;
using System.Diagnostics.Contracts;

namespace BlueBit.CarsEvidence.GUI.Desktop.Services
{
    public interface IEnviromentService
    {
        DateTime CurrentDate { get; }
    }

    public static class EnviromentServiceExtensions
    {
        public static int GetCurrentYear(this IEnviromentService @this)
        {
            Contract.Assert(@this != null);
            return @this.CurrentDate.Year;
        }
        public static byte GetCurrentMonth(this IEnviromentService @this)
        {
            Contract.Assert(@this != null);
            return (byte)@this.CurrentDate.Month;
        }
    }

    [Register(typeof(IEnviromentService))]
    public class EnviromentService :
        IEnviromentService
    {
        public DateTime CurrentDate { get { return DateTime.Now.Date; } }
    }
}
