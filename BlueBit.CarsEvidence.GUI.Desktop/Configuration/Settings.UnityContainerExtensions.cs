using BlueBit.CarsEvidence.Commons.Reflection;
using BlueBit.CarsEvidence.Commons.Templates;
using BlueBit.CarsEvidence.GUI.Desktop.Model;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Interactions;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Dialogs;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Documents;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Documents.Commands.Handlers;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Panels;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Panels.Commands;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Panels.Commands.Handlers;
using dotNetExt;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using _Attributes = BlueBit.CarsEvidence.GUI.Desktop.Model.Attributes;
using _Objects = BlueBit.CarsEvidence.GUI.Desktop.Model.Objects;

namespace BlueBit.CarsEvidence.GUI.Desktop.Configuration
{
    internal static partial class UnityContainerExtensions
    {
        public static IUnityContainer RegisterModel(this IUnityContainer container, Func<LifetimeManager> singletonLifeTimeMgr, IEnumerable<Type> types)
        {
            types
                .Where(Commons.Reflection.TypeExtensions.HasAttribute<Attributes.RegisterAttribute>)
                .Each(type =>
                    {
                        var register = type.GetAttribute<Attributes.RegisterAttribute>();
                        if (register.AsSingleton)
                            container.RegisterType(register.AsType, type, singletonLifeTimeMgr());
                        else
                            container.RegisterType(register.AsType, type);
                    });
            types
                .Where(Commons.Reflection.TypeExtensions.HasAttribute<Attributes.RegisterAllAttribute>)
                .Each(baseType =>
                {
                    var register = baseType.GetAttribute<Attributes.RegisterAllAttribute>();
                    baseType
                        .GetImplementingTypes(types)
                        .Each(type =>
                        {
                            var regName = string.Format("{0}#{1}", baseType.Name, type.FullName);

                            if (register.AsSingleton)
                                container.RegisterType(baseType, type, regName, singletonLifeTimeMgr());
                            else
                                container.RegisterType(baseType, type, regName);
                        });
                    {
                        var type = typeof(IEnumerable<>).MakeGenericType(baseType);
                        container
                            .RegisterType(type, new InjectionFactory(_ => _.ResolveAll(baseType)));
                    }
                });

            var generalTypes = types
                .GetDerivedTypes<_Objects.View.General.ViewGeneralObjectBase>()
                .ToDictionary(t => t.Name);
            var viewTypes = types
                .GetDerivedTypes<_Objects.View.ViewObjectBase>()
                .Where(Commons.Reflection.TypeExtensions.HasAttribute<_Attributes.ShowInPanelViewAttribute>)
                .ToDictionary(t => t.Name);
            var editTypes = types
                .GetDerivedTypes<_Objects.Edit.EditObjectBase>()
                .Where(Commons.Reflection.TypeExtensions.HasAttribute<_Attributes.EditInDocumentViewAttribute>)
                .ToDictionary(t => t.Name);
            var editAsChildTypes = types
                .GetDerivedTypes<_Objects.Edit.EditObjectBase>()
                .Where(Commons.Reflection.TypeExtensions.HasAttribute<_Attributes.EditInDocumentViewAsChildAttribute>)
                .ToDictionary(t => t.Name);

            return container
                .RegisterModelForGeneral(singletonLifeTimeMgr, generalTypes.Values)
                .RegisterModelForView(singletonLifeTimeMgr, viewTypes.Values, types)
                .RegisterModelForEdit(singletonLifeTimeMgr, editTypes.Values, types)
                .RegisterModelForEditAsChild(singletonLifeTimeMgr, editAsChildTypes.Values)
                .RegisterModelForDialog(singletonLifeTimeMgr, types)
                .RegisterType<IEnumerable<IShowCommandHandler>>(new InjectionFactory(_ => _.ResolveAll<IShowCommandHandler>()))
                .RegisterType<IEnumerable<IAddCommandHandler>>(new InjectionFactory(_ => _.ResolveAll<IAddCommandHandler>()))
                .RegisterType<IEnumerable<IEditAllCommandHandler>>(new InjectionFactory(_ => _.ResolveAll<IEditAllCommandHandler>()))
                .RegisterType<MainWindowViewModel>(singletonLifeTimeMgr())
                ;
        }

        public static IUnityContainer RegisterModelFor<T>(this IUnityContainer container, IEnumerable<Type> types)
        {
            foreach (var type in types.GetDerivedTypes<T>())
                container.RegisterType(type);
            return container;
        }

        public static IUnityContainer RegisterModelForDialog(this IUnityContainer container, Func<LifetimeManager> singletonLifeTimeMgr, IEnumerable<Type> types)
        {
            foreach (var type in types.GetDerivedTypes<DialogViewModelBase>())
            {
                var serviceTypeBase = typeof(IDialogService<>).MakeGenericType(type);
                var serviceTypeImpl = typeof(DialogService<>).MakeGenericType(type);
                container
                    .RegisterType(type)
                    .RegisterType(serviceTypeBase, serviceTypeImpl, singletonLifeTimeMgr());
            }
            return container;
        }

        public static IUnityContainer RegisterModelForGeneral(this IUnityContainer container, Func<LifetimeManager> singletonLifeTimeMgr, IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                var entityType = type.GetAttribute<_Attributes.EntityTypeAttribute>().EntityType;
                var converterTypeBase = typeof(_Objects.IConverterFromEntity<,>);
                var converterTypeBaseGen = converterTypeBase.MakeGenericType(type, entityType);
                var converterType = GetConverterType(type, entityType);
                var converterTypeInst = typeof(_Objects.ConverterInstance<>).MakeGenericType(converterType);
                var repositoryViewObjectsType = typeof(IRepositoryViewObjects<>).MakeGenericType(type);
                var repositoryViewObjectsMethodType = repositoryViewObjectsType.GetMethods().Single(); //GetViewObjects()
                var viewObjectsType = typeof(IViewObjects<>).MakeGenericType(type);
                var generalObjectsType = typeof(IGeneralObjects<>).MakeGenericType(type);

                Func<IUnityContainer, object> getViewObjects = (c) =>
                {
                    var obj = c.Resolve(repositoryViewObjectsType);
                    return repositoryViewObjectsMethodType.Invoke(obj, null);
                };

                container
                    .RegisterType(type)
                    .RegisterType(repositoryViewObjectsType, CreateFactory<Repositories>())
                    .RegisterType(viewObjectsType, new InjectionFactory(getViewObjects))
                    .RegisterType(generalObjectsType, new InjectionFactory(getViewObjects))
                    .RegisterType(converterType)
                    .RegisterType(converterTypeInst, singletonLifeTimeMgr())
                    .RegisterType(converterTypeBaseGen, CreateConverterFactory(converterTypeInst))
                    ;
            }
            return container;
        }

        public static IUnityContainer RegisterModelForView(this IUnityContainer container, Func<LifetimeManager> singletonLifeTimeMgr, IEnumerable<Type> types, IEnumerable<Type> allTypes)
        {
            foreach (var type in types)
            {
                var entityType = type.GetAttribute<_Attributes.EntityTypeAttribute>().EntityType;
                var converterTypeBase = typeof(_Objects.IConverterFromEntity<,>);
                var converterTypeBaseGen = converterTypeBase.MakeGenericType(type, entityType);
                var converterType = GetConverterType(type, entityType);
                var converterTypeInst = typeof(_Objects.ConverterInstance<>).MakeGenericType(converterType);
                var repositoryViewObjectsType = typeof(IRepositoryViewObjects<>).MakeGenericType(type);
                var repositoryViewObjectsMethodType = repositoryViewObjectsType.GetMethods().Single(); //GetViewObjects()
                var viewObjectsType = typeof(IViewObjects<>).MakeGenericType(type);
                var generalObjectsType = typeof(IGeneralObjects<>).MakeGenericType(type);

                Func<IUnityContainer, object> getViewObjects = (c) =>
                {
                    var obj = c.Resolve(repositoryViewObjectsType);
                    return repositoryViewObjectsMethodType.Invoke(obj, null);
                };

                container
                    .RegisterType(type)
                    .RegisterType(repositoryViewObjectsType, CreateFactory<Repositories>())
                    .RegisterType(viewObjectsType, new InjectionFactory(getViewObjects))
                    .RegisterType(generalObjectsType, new InjectionFactory(getViewObjects))
                    .RegisterType(converterType)
                    .RegisterType(converterTypeInst, singletonLifeTimeMgr())
                    .RegisterType(converterTypeBaseGen, CreateConverterFactory(converterTypeInst))
                    ;

                {
                    var panelType = typeof(PanelViewModelBase);
                    var panelTypeBase = typeof(ListPanelViewModelBase<>);
                    var panelTypeBaseGen = panelTypeBase.MakeGenericType(type);
                    var panelTypeImpl = allTypes.GetDerivedTypes(panelTypeBaseGen).Single();
                    var creatorTypeBase = typeof(IListPanelViewModelCreator<>);
                    var creatorTypeImpl = typeof(ListPanelViewModelCreator<,>);
                    var creatorTypeBaseGen = creatorTypeBase.MakeGenericType(panelTypeBaseGen);
                    var creatorTypeImplGen = creatorTypeImpl.MakeGenericType(panelTypeBaseGen, type);

                    var regName = string.Format("{0}${1}", panelType.Name, panelTypeImpl.FullName);
                    container
                        .RegisterType(panelType, panelTypeImpl, regName)
                        .RegisterType(panelTypeBaseGen, panelTypeImpl)
                        .RegisterType(creatorTypeBaseGen, creatorTypeImplGen);
                }
                {
                    var cmdType = typeof(IShowCommandHandler);
                    var cmdTypeBase = typeof(IShowCommandHandler<>);
                    var cmdTypeBaseGen = cmdTypeBase.MakeGenericType(type);
                    var cmdTypeImpl = typeof(ShowCommandHandler<>);
                    var cmdTypeImplGen = cmdTypeImpl.MakeGenericType(type);
                    var regName = string.Format("{0}#{1}", cmdType.Name, type.FullName);
                    container
                        .RegisterType(cmdTypeBaseGen, cmdTypeImplGen, singletonLifeTimeMgr())
                        .RegisterType(cmdType, regName, new InjectionFactory(_ => _.Resolve(cmdTypeBaseGen)));
                }
            }
            return container;
        }

        public static IUnityContainer RegisterModelForEdit(this IUnityContainer container, Func<LifetimeManager> singletonLifeTimeMgr, IEnumerable<Type> types, IEnumerable<Type> allTypes)
        {
            foreach (var type in types)
            {
                var entityType = type.GetAttribute<_Attributes.EntityTypeAttribute>().EntityType;
                var converterFromTypeBase = typeof(_Objects.IConverterFromEntity<,>);
                var converterFromTypeBaseGen = converterFromTypeBase.MakeGenericType(type, entityType);
                var converterToTypeBase = typeof(_Objects.IConverterToEntity<,>);
                var converterToTypeBaseGen = converterToTypeBase.MakeGenericType(type, entityType);
                var converterType = GetConverterType(type, entityType);
                var converterTypeInst = typeof(_Objects.ConverterInstance<>).MakeGenericType(converterType);
                var repositoryEditObjectsType = typeof(IRepositoryEditObjects<>).MakeGenericType(type);
                var repositoryEditObjectsMethodType = repositoryEditObjectsType.GetMethods().Single(); //GetEditObjects()
                var editObjectsType = typeof(IEditObjects<>).MakeGenericType(type);
                var generalObjectsType = typeof(IGeneralObjects<>).MakeGenericType(type);

                Func<IUnityContainer, object> getEditObjects = (c) =>
                {
                    var obj = c.Resolve(repositoryEditObjectsType);
                    return repositoryEditObjectsMethodType.Invoke(obj, null);
                };

                container
                    .RegisterType(type)
                    .RegisterType(repositoryEditObjectsType, CreateFactory<Repositories>())
                    .RegisterType(editObjectsType, new InjectionFactory(getEditObjects))
                    .RegisterType(generalObjectsType, new InjectionFactory(getEditObjects))
                    .RegisterType(converterType)
                    .RegisterType(converterTypeInst, singletonLifeTimeMgr())
                    .RegisterType(converterFromTypeBaseGen, CreateConverterFactory(converterTypeInst))
                    .RegisterType(converterToTypeBaseGen, CreateConverterFactory(converterTypeInst))
                    ;

                {
                    var editDocType = typeof(EditDocumentViewModelBase);
                    var editDocTypeBase = typeof(EditDocumentViewModelBase<>);
                    var editDocTypeBaseGen = editDocTypeBase.MakeGenericType(type);
                    var editDocTypeImpl = allTypes.GetDerivedTypes(editDocTypeBaseGen).Single();
                    var creatorTypeBase = typeof(IEditDocumentViewModelCreator<,>);
                    var creatorTypeImpl = typeof(EditDocumentViewModelCreator<,>);
                    var creatorTypeBaseGen = creatorTypeBase.MakeGenericType(editDocTypeBaseGen, type);
                    var creatorTypeImplGen = creatorTypeImpl.MakeGenericType(editDocTypeBaseGen, type);

                    var regName = string.Format("{0}#{1}", editDocType.Name, type.FullName);
                    container
                        .RegisterType(editDocType, editDocTypeImpl, regName)
                        .RegisterType(editDocTypeBaseGen, editDocTypeImpl)
                        .RegisterType(creatorTypeBaseGen, creatorTypeImplGen);
                }

                var singleEntity = type.HasAttribute<_Attributes.CanEditAllInDocumentViewAttribute>();
                {
                    new Tuple<Type, Type, Type, bool, bool>[] {
                        Tuple.Create(typeof(IAddCommandHandler), typeof(IAddCommandHandler<>), typeof(AddCommandHandler<>), true, !singleEntity),
                        Tuple.Create(typeof(IEditAllCommandHandler), typeof(IEditAllCommandHandler<>), typeof(EditAllCommandHandler<>), true, singleEntity),
                    }
                    .Where(_ => _.Item5)
                    .Each(_ =>
                    {
                        var cmdType = _.Item1;
                        var cmdTypeBase = _.Item2;
                        var cmdTypeImpl = _.Item3;
                        var cmdTypeBaseGen = cmdTypeBase.MakeGenericType(type);
                        var cmdTypeImplGen = cmdTypeImpl.MakeGenericType(type);
                        var regName = string.Format("{0}#{1}", cmdType.Name, type.FullName);
                        var r = (_.Item4
                            ? container.RegisterType(cmdTypeBaseGen, cmdTypeImplGen, singletonLifeTimeMgr())
                            : container.RegisterType(cmdTypeBaseGen, cmdTypeImplGen))
                            .RegisterType(cmdType, regName, new InjectionFactory(c => c.Resolve(cmdTypeBaseGen)));
                    });
                }
                {
                    new Tuple<Type, Type, bool, bool>[] {
                            Tuple.Create(typeof(IOpenEditDocumentCommandHelper<>), typeof(OpenEditDocumentCommandHelper<>), true, true),
                            Tuple.Create(typeof(IEditCommandHandler<>), typeof(EditCommandHandler<>), true, true),
                            Tuple.Create(typeof(IDeleteCommandHandler<>), typeof(DeleteCommandHandler<>), true, !singleEntity),
                            Tuple.Create(typeof(IApplyCommandHandler<>), typeof(ApplyCommandHandler<>), false, true),
                            Tuple.Create(typeof(ICancelCommandHandler<>), typeof(CancelCommandHandler<>), false, true),
                        }
                    .Where(_ => _.Item4)
                    .Each(_ =>
                    {
                        var cmdTypeBase = _.Item1;
                        var cmdTypeImpl = _.Item2;
                        var cmdTypeBaseGen = cmdTypeBase.MakeGenericType(type);
                        var cmdTypeImplGen = cmdTypeImpl.MakeGenericType(type);
                        var r = _.Item3
                            ? container.RegisterType(cmdTypeBaseGen, cmdTypeImplGen, singletonLifeTimeMgr())
                            : container.RegisterType(cmdTypeBaseGen, cmdTypeImplGen);
                    });
                }
            }
            return container;
        }

        public static IUnityContainer RegisterModelForEditAsChild(this IUnityContainer container, Func<LifetimeManager> singletonLifeTimeMgr, IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                var entityType = type.GetAttribute<_Attributes.EntityTypeAttribute>().EntityType;
                var parentType = type.GetAttribute<_Attributes.EditInDocumentViewAsChildAttribute>().ParentType;
                var parentEntityType = parentType.GetAttribute<_Attributes.EntityTypeAttribute>().EntityType;
                var converterFromTypeBase = typeof(_Objects.IConverterFromEntityChild<,,>);
                var contextType = typeof(Tuple<,>).MakeGenericType(parentType, parentEntityType);
                var converterFromTypeBaseGen = converterFromTypeBase.MakeGenericType(contextType, type, entityType);
                var converterToTypeBase = typeof(_Objects.IConverterToEntityChild<,,>);
                var converterToTypeBaseGen = converterToTypeBase.MakeGenericType(contextType, type, entityType);
                var converterType = GetConverterType(type, entityType);
                var converterTypeInst = typeof(_Objects.ConverterInstance<>).MakeGenericType(converterType);

                container
                    .RegisterType(type)
                    .RegisterType(converterType)
                    .RegisterType(converterTypeInst, singletonLifeTimeMgr())
                    .RegisterType(converterFromTypeBaseGen, CreateConverterFactory(converterTypeInst))
                    .RegisterType(converterToTypeBaseGen, CreateConverterFactory(converterTypeInst))
                    ;
            }
            return container;
        }
    }
}
