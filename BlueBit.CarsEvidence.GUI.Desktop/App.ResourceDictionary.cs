using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace BlueBit.CarsEvidence.GUI.Desktop
{
    partial class App
    {
        public static readonly IReadOnlyDictionary<EntityType, int> VisibleTypesOrder = new EntityType[]
            {
                EntityType.Person,
                EntityType.Car,
                EntityType.Address,
                EntityType.Route,
                EntityType.Period,
            }
            .Select((v, i) => new { v, i })
            .ToDictionary(_ => _.v, _ => _.i);

        public static class ResourceDictionary
        {
            public const string StrApp = "strApp";
            public const string StrFilterXML = "strFilterXML";

            public const string StrObject = "strObject_{0}";
            public const string StrObjectFrmt = "strObject_{0}_Frmt";
            public const string ImgObject = "imgObject_{0}";

            public const string StrObjects = "strObjects_{0}";
            public const string StrObjectsFrmt = "strObjects_{0}_Frmt";
            public const string ImgObjects = "imgObjects_{0}";

            public const string StrRibbonGroups = "strApp_RibbonGroup_{0}";
            public const string ImgRibbonGroups = "imgApp_RibbonGroup_{0}";

            public const string StrCmd = "strCmd_{0}";
            public const string ImgCmd_S = "imgCmd_s_{0}";
            public const string ImgCmd_N = "imgCmd_n_{0}";
            public const string ImgCmd_L = "imgCmd_l_{0}";

            public static T GetResource<T>(string key)
            {
                return (T)App.Current.FindResource(key);
            }

        }
    }
}
