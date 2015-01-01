using BlueBit.CarsEvidence.Commons.Linq;
using BlueBit.CarsEvidence.Commons.Reflection;
using BlueBit.CarsEvidence.Commons.Templates;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Documents;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Panels;
using Microsoft.Practices.Unity;
using System;
using System.Linq;
using _Attributes = BlueBit.CarsEvidence.GUI.Desktop.Model.Attributes;
using _Objects = BlueBit.CarsEvidence.GUI.Desktop.Model.Objects;

namespace BlueBit.CarsEvidence.GUI.Desktop.Configuration
{
    partial class UnityContainerExtensions
    {
        private class _ListPanelViewModelCreator<T, TItem> :
            ISingletonCreator<T>
            where T : ListPanelViewModelBase<TItem>
            where TItem : _Objects.View.ViewObjectBase
        {
            private readonly Func<MainWindowViewModel> _model;
            private readonly Func<T> _creator;

            public _ListPanelViewModelCreator(
                Func<MainWindowViewModel> model,
                Func<T> creator)
            {
                _model = model;
                _creator = creator;
            }

            public T Create()
            {
                var panel = _creator();
                _model().AddPanelViewModel(panel);
                return panel;
            }

            public T GetInstance()
            {
                return _model().PanelViewModels.Castable<T>().FirstOrDefault();
            }
        }

        private class _EditDocumentViewModelCreator<T, TItem> :
            ISingletonCreatorForItem<T, TItem>
            where T : EditDocumentViewModelBase<TItem>
            where TItem : _Objects.Edit.EditObjectBase
        {
            private readonly Func<MainWindowViewModel> _model;
            private readonly Func<T> _creator;

            public _EditDocumentViewModelCreator(
                Func<MainWindowViewModel> model,
                Func<T> creator)
            {
                _model = model;
                _creator = creator;
            }

            public T Create(TItem item)
            {
                var document = _creator();
                document.Item = item;
                _model().AddDocumentViewModel(document);
                item.Validate();
                return document;
            }

            public T GetInstance(TItem item)
            {
                return _model().DocumentViewModels.Castable<T>().FirstOrDefault(d => d.Item.Equals(item));
            }
        }

        private static Type GetConverterType(Type type, Type entityType)
        {
            var converterType = type.GetAttribute<_Attributes.ConverterTypeAttribute>().ConverterType;
            if (converterType.IsGenericTypeDefinition)
            {
                converterType = converterType.MakeGenericType(type, entityType);
            }
            return converterType;
        }

        private static Type GetConverterType<T>(Type entityType)
            where T : _Objects.ObjectBase
        {
            return GetConverterType(typeof(T), entityType);
        }


        private static InjectionMember CreateConverterFactory(Type type)
        {
            return new InjectionFactory(c =>
            {
                var instance = (_Objects.IConverterInstance)c.Resolve(type);
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
