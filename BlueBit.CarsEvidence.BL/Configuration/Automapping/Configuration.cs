using BlueBit.CarsEvidence.BL.Entities.Components;
using BlueBit.CarsEvidence.BL.Entities.UserTypes;
using BlueBit.CarsEvidence.Commons.Reflection;
using FluentNHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlueBit.CarsEvidence.BL.Configuration.Automapping
{
    public static class Configuration
    {
        const string tableNameFormat = "T_{0}";

        private class _DefaultAutomappingConfiguration : DefaultAutomappingConfiguration
        {
            public override bool ShouldMap(Type type)
            {
                return type.IsDerivedFrom<Repositories.ObjectInRepositoryBase>()
                    || type.IsDerivedFrom<Repositories.ObjectChildInRepositoryBase>()
                    || type.IsDerivedFrom<Repositories.ComponentBase>();
            }

            public override bool IsComponent(Type type)
            {
                if (type.IsDerivedFrom<Repositories.ComponentBase>())
                    return true;

                return base.IsComponent(type);
            }

            public override string GetComponentColumnPrefix(Member member)
            {
                var prefix = base.GetComponentColumnPrefix(member);
                return prefix + "_";
            }
        }

        private class _CommonConventions :
            IClassConvention,
            IIdConvention,
            IPropertyConvention, 
            IPropertyConventionAcceptance
        {
            public void Apply(IClassInstance instance)
            {
                System.Diagnostics.Debug.WriteLine("NHClassIns:[{0}]",
                    instance.EntityType);

                var tableName = Inflector.Inflector.Pluralize(instance.EntityType.Name);
                instance.Table(string.Format(tableNameFormat, tableName));
            }

            public void Apply(IIdentityInstance instance)
            {
                System.Diagnostics.Debug.WriteLine("NHIdIns:[{0}].[{1}]",
                    instance.EntityType,
                    instance.Property.MemberInfo.Name);

                var tableColumnNameWithID = Commons.Reflection.PropertyHelper<Repositories.IObjectInRepository>.GetPropertyName(obj => obj.ID);
                instance.Column(tableColumnNameWithID);
                instance.GeneratedBy.Native();
            }

            public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
            {
                criteria.Expect(c => {
                    var memberInfo = c.Property.MemberInfo;
                    return memberInfo.HasAttribute<RequiredAttribute>()
                        || memberInfo.HasAttribute<MaxLengthAttribute>()
                        || memberInfo.IsPropertyType<string>();
                });
            }
            public void Apply(IPropertyInstance instance)
            {
                System.Diagnostics.Debug.WriteLine("NHPropIns:[{0}].[{1}]",
                    instance.EntityType,
                    instance.Property.MemberInfo.Name);

                var memberInfo = instance.Property.MemberInfo
                    .OnAttribute<RequiredAttribute>((_, attr) => instance.Not.Nullable())
                    .OnAttribute<MaxLengthAttribute>((_, attr) => instance.Length(attr.Length))
                    .OnPropertyType<string>((_) => instance.CustomType<Text>());
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
