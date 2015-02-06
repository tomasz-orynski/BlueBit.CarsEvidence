using BlueBit.CarsEvidence.BL.Repositories;
using BlueBit.CarsEvidence.Commons.Linq;
using BlueBit.CarsEvidence.Commons.Templates;
using BlueBit.CarsEvidence.GUI.Desktop.Model;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Documents;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Panels;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using _Objects = BlueBit.CarsEvidence.GUI.Desktop.Model.Objects;
using _Attributes = BlueBit.CarsEvidence.GUI.Desktop.Model.Attributes;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Documents.Commands;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Panels.Commands;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Dialogs;

namespace BlueBit.CarsEvidence.GUI.Desktop.Configuration
{
    internal static class Settings
    {
        private static readonly IUnityContainer container = RegisterTypes();

        private static IUnityContainer RegisterTypes()
        {
            var assemblyBL = typeof(BL.Configuration.Settings).Assembly;
            var assemblyBLTypes = assemblyBL.GetExportedTypes();
            var assemblyGUI = typeof(Configuration.Settings).Assembly;
            var assemblyGUITypes = assemblyGUI.GetExportedTypes();

            Func<LifetimeManager> getLifeTimeMgr = () => new ContainerControlledLifetimeManager();
            return new UnityContainer()
                .RegisterType<ISessionFactory>(new InjectionFactory((c) => BL.Configuration.Settings.CreateSessionFactory()))
                .RegisterType<ISession>(new InjectionFactory((c) => c.Resolve<ISessionFactory>().OpenSession()))
                .RegisterType<IDbRepositories, DbRepositories>(getLifeTimeMgr())
                .RegisterType<Repositories, Repositories>(getLifeTimeMgr())
                .RegisterModel(getLifeTimeMgr, assemblyGUITypes);
        }

        public static void Init()
        {
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));
        }

        public static T ResolveType<T>()
        {
            return container.Resolve<T>();
        }
        public static IEnumerable<T> ResolveTypes<T>()
        {
            return container.ResolveAll<T>();
        }
        public static object ResolveType(Type type)
        {
            return container.Resolve(type);
        }
        public static IEnumerable<object> ResolveTypes(Type type)
        {
            return container.ResolveAll(type);
        }
    }
}
