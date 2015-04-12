using BlueBit.CarsEvidence.BL.Entities.Attributes;
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
            IIdConvention
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
        }

        private class _ForeignKeyConvention :
            ForeignKeyConvention
        {
            protected override string GetKeyName(Member property, Type type)
            {
                var tableColumnNameWithID = Commons.Reflection.PropertyHelper<Entities.IEntity>.GetPropertyName(obj => obj.ID);
                return property == null
                        ? type.Name + tableColumnNameWithID
                        : property.Name + tableColumnNameWithID;
            }
        }

        private class _RequiredAttributeConvention :
            AttributePropertyConvention<RequiredAttribute>
        {
            protected override void Apply(RequiredAttribute attribute, IPropertyInstance instance)
            {
                instance.Not.Nullable();
            }
        }
        private class _MaxLengthAttributeConvention :
            AttributePropertyConvention<MaxLengthAttribute>
        {
            protected override void Apply(MaxLengthAttribute attribute, IPropertyInstance instance)
            {
                instance.Length(attribute.Length);
            }
        }

        private class _PrecisionScaleAttributeConvention :
            AttributePropertyConvention<PrecisionScaleAttribute>
        {
            protected override void Apply(PrecisionScaleAttribute attribute, IPropertyInstance instance)
            {
                instance.Precision(attribute.Precision);
                instance.Scale(attribute.Scale);
            }
        }

        private class _TextTypeConvention :
            UserTypeConvention<Text>
        {
            public override void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
            {
                criteria.Expect(c => {
                    var memberInfo = c.Property.MemberInfo;
                    return memberInfo.IsPropertyType<string>();
                });
            }
        }

        private class _EnumTypeConvention :
            IUserTypeConvention
        {
            public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
            {
                criteria.Expect(e => e.Property.PropertyType.IsEnum);
            }

            public void Apply(IPropertyInstance instance)
            {
                instance.CustomType(instance.Property.PropertyType);
            }
        }

        public static DefaultAutomappingConfiguration DefaultAutomappingConfiguration { get { return new _DefaultAutomappingConfiguration(); }}
        public static IEnumerable<IConvention> Conventions 
        { 
            get { 
                yield return new _CommonConventions(); 
                yield return new _ForeignKeyConvention();
                yield return new _RequiredAttributeConvention();
                yield return new _MaxLengthAttributeConvention();
                yield return new _TextTypeConvention();
                yield return new _EnumTypeConvention();
                yield return new _PrecisionScaleAttributeConvention();
            }
        }
    }
}
