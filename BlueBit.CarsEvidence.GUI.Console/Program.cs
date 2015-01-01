﻿using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueBit.CarsEvidence.Console
{
    partial class Program
    {
        const string connectionStringKey = "CarsEvidence";

        static void Main(string[] args)
        {
            foreach (var arg in args)
                switch (arg)
                {
                    case "-createschema":
                    case "-cs":
                        CreateSchema();
                        break;

                    case "-createdata":
                    case "-cd":
                        CreateData();
                        break;
                }

            SelectData();
        }

        static void CreateSchema()
        {
            try
            {
                BL.Configuration.Settings.RecreateSchema(connectionStringKey);
            }
            catch (Exception e)
            {
                System.Console.Out.WriteLine(e.Message);
            }
        }

        static void SelectData()
        {
            using (var sessionFactory = BL.Configuration.Settings.CreateSessionFactory(connectionStringKey))
            using (var session = sessionFactory.OpenSession())
            {
                BL.Entities.Period periodAlias = null;
                BL.Entities.PeriodEntry periodEntryAlias = null;
                BL.Entities.Person personAlias = null;

                var queryResult = session
                    .QueryOver<BL.Entities.PeriodEntry>(() => periodEntryAlias)
                    .JoinAlias(() => periodEntryAlias.Period, () => periodAlias)
                    .JoinAlias(() => periodEntryAlias.Person, () => personAlias)
                    .Where(() => periodAlias.Year == 2014 && periodAlias.Month == 7)
                    .Fetch(pe => pe.Period).Eager
                    .Fetch(pe => pe.Person).Eager
                    .Fetch(pe => pe.Route).Eager
                    .Fetch(pe => pe.Route.AddressFrom).Eager
                    .Fetch(pe => pe.Route.AddressTo).Eager
                    .TransformUsing(Transformers.DistinctRootEntity)
                    .List();
            }
        }
    }
}
