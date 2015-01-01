using FluentNHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlueBit.CarsEvidence.Commons.Reflection;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueBit.CarsEvidence.BL.Configuration
{
    public static class Settings
    {
        private static AutoPersistenceModel OverrideAllTypes(this AutoPersistenceModel model)
        {
            var cfgTypeGen = typeof(Entities.IEntityCfg);
            var cfgs = cfgTypeGen.Assembly.GetTypes()
                .Where(t => t.GetInterfaces().Any(_ => _ == cfgTypeGen) && !t.IsAbstract && !t.IsGenericType)
                .Select(t => (Entities.IEntityCfg)Activator.CreateInstance(t));

            model.OverrideAll(m => m.IgnoreProperties(p => p.MemberInfo.HasAttribute<NotMappedAttribute>()));

            foreach (var cfg in cfgs)
                cfg.Configure(model);

            return model;
        }

        private static FluentConfiguration CreateConfiguration(string connectionStringKey)
        {
            return Fluently
                .Configure()
                .Database(MsSqlConfiguration.MsSql2012.ConnectionString(c => c.FromConnectionStringWithKey(connectionStringKey)))
                .Mappings(m => m
                    .AutoMappings
                    .Add(AutoMap.AssemblyOf<Entities.IEntity>(Automapping.Configuration.DefaultAutomappingConfiguration)
                        .OverrideAllTypes()
                        .Conventions.Add(Automapping.Configuration.Conventions.ToArray())
                        )
                    );
        }

        public static void RecreateSchema(string connectionStringKey)
        {
            var config = CreateConfiguration(connectionStringKey)
                .ExposeConfiguration(cfg =>
                {
                    var schema = new SchemaExport(cfg);
                    schema.Drop(false, true);
                    schema.Create(false, true);
                })
                .BuildConfiguration();

            using (var sessionFactory = config.BuildSessionFactory())
            using (var session = sessionFactory.OpenSession())
            {
            }
        }

        public static ISessionFactory CreateSessionFactory(string connectionStringKey)
        {
            return CreateConfiguration(connectionStringKey)
                .BuildSessionFactory();
        }
    }
}
