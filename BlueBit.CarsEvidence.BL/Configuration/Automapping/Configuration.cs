using FluentNHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlueBit.CarsEvidence.BL.Configuration.Automapping
{
    public static class Configuration
    {
        const string tableNameFormat = "T_{0}";

        private class _DefaultAutomappingConfiguration : DefaultAutomappingConfiguration
        {
            public override bool ShouldMap(Type type)
            {
                return type.IsSubclassOf(typeof(Repositories.ObjectInRepositoryBase))
                    || type.IsSubclassOf(typeof(Repositories.ObjectChildInRepositoryBase));
            }
        }

        private class _CommonConventions :
            IClassConvention,
            IIdConvention,
            IPropertyConvention, 
            IPropertyConventionAcceptance
        {
            public void Apply(FluentNHibernate.Conventions.Instances.IClassInstance instance)
            {
                var tableName = Inflector.Inflector.Pluralize(instance.EntityType.Name);
                instance.Table(string.Format(tableNameFormat, tableName));
            }

            public void Apply(FluentNHibernate.Conventions.Instances.IIdentityInstance instance)
            {
                var tableColumnNameWithID = Commons.Reflection.PropertyHelper<Repositories.IObjectInRepository>.GetPropertyName(obj => obj.ID);
                instance.Column(tableColumnNameWithID);
                instance.GeneratedBy.Native();
            }

            public void Apply(IPropertyInstance instance)
            {
                instance.Not.Nullable();
            }

            public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
            {
                criteria.Expect(c => c.Property.MemberInfo.IsDefined(typeof(RequiredAttribute), false));
            }
        }

        private class _ForeignKeyConvention : ForeignKeyConvention
        {
            protected override string GetKeyName(Member property, Type type)
            {
                var tableColumnNameWithID = Commons.Reflection.PropertyHelper<Entities.IEntity>.GetPropertyName(obj => obj.ID);
                return property == null
                        ? type.Name + tableColumnNameWithID
                        : property.Name + tableColumnNameWithID;
            }
        }

        public static DefaultAutomappingConfiguration DefaultAutomappingConfiguration { get { return new _DefaultAutomappingConfiguration(); }}
        public static IEnumerable<IConvention> Conventions 
        { 
            get { 
                yield return new _CommonConventions(); 
                yield return new _ForeignKeyConvention();
            } 
        }
    }
}
