using BlueBit.CarsEvidence.Commons.Reflection;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Attributes;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects;
using Microsoft.Practices.Unity;
using System;

namespace BlueBit.CarsEvidence.GUI.Desktop.Configuration
{
    partial class UnityContainerExtensions
    {
        private static Type GetConverterType(Type type, Type entityType)
        {
            var converterType = type.GetAttribute<ConverterTypeAttribute>().ConverterType;
            if (converterType.IsGenericTypeDefinition)
            {
                converterType = converterType.MakeGenericType(type, entityType);
            }
            return converterType;
        }

        private static Type GetConverterType<T>(Type entityType)
            where T : ObjectBase
        {
            return GetConverterType(typeof(T), entityType);
        }

        private static InjectionMember CreateConverterFactory(Type type)
        {
            return new InjectionFactory(c =>
            {
                var instance = (IConverterInstance)c.Resolve(type);
                return instance.Instance;
            });
        }

        public static InjectionFactory CreateFactory<T>()
        {
            Func<IUnityContainer, object> resolve = (c) =>
            {
                var obj = c.Resolve<T>();
                return obj;
            };
            return new InjectionFactory(resolve);
        }
    }
}
