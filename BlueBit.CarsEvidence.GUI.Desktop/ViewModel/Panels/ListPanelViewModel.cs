using BlueBit.CarsEvidence.Commons.Reflection;
using BlueBit.CarsEvidence.Commons.Templates;
using BlueBit.CarsEvidence.GUI.Desktop.Model;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit.Documents;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.View;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.View.Panels;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Commands;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Panels.Commands.Handlers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Windows.Input;
using ObjectsEditDocuments = BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit.Documents;
using ObjectsViewPanels = BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.View.Panels;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Panels
{
    public abstract class ListPanelViewModelBase : 
        PanelViewModelBase,
        IObjectWithCommands
    {
        public ICommand EventCmdKeyInsertPressed { get { return _commands[CmdKey.Add].Command; } }
        public ICommand EventCmdKeyDeletePressed { get { return _commands[CmdKey.Delete].Command; } }
        public ICommand EventCmdKeyEnterPressed { get { return _commands[CmdKey.Edit].Command; } }
        public ICommand EventCmdMouseDoubleClick { get { return _commands[CmdKey.Edit].Command; } }
        public abstract ICommand EventCmdSelectectionChanged { get; }

        //TODO - przerobić na opóźnione tworzenie...
        private readonly CommandsViewModel _commands = new CommandsViewModel();
        public CommandsViewModel Commands { get { return _commands; } }

        public abstract ISelectionInfo ItemsSelected { get; }
    }

    public interface IListPanelViewModel<T> :
        IObjectWithItems<ObservableCollection<T>>,
        IObjectForEntityType
        where T : ViewPanelObjectBase
    {
    }

    public abstract class ListPanelViewModelBase<T> : 
        ListPanelViewModelBase,
        IListPanelViewModel<T>
        where T: ViewPanelObjectBase
    {
        protected readonly IViewObjects<T> _viewObjects;
        private readonly Lazy<ISelectionCommand<T>> _cmdSelected = CreateCommandForSelectionLazy<T>();

        public override ICommand EventCmdSelectectionChanged { get { return _cmdSelected.Value; } }

        public ObservableCollection<T> Items { get { return _viewObjects.Items; } }
        public override ISelectionInfo ItemsSelected { get { return _cmdSelected.Value; } }

        public EntityType ForType { get { return EntityTypeDict.GetValueForObjectType<T>(); } }

        public override sealed string Title { get { return this.GetTitle(); } }

        public ListPanelViewModelBase(
            IViewObjects<T> viewObjects
            )
        {
            _viewObjects = viewObjects;
        }

        private IEnumerable<ObjectBase> GetSelectedSet() { return _cmdSelected.Value.SelectedSet; }
        protected CommandInfo CreateCommandForSelected<TCmd>(CmdKey key, TCmd cmd)
            where TCmd: ICommandHandlerForSelected<ObjectBase>
        {
            cmd.SelectedSet = GetSelectedSet;
            return _CreateCommand(key, cmd.Execute, cmd.CanExecute);
        }
    }

    public class ListPanelViewModelBaseParamSet<TView, TEdit>
        where TView : ViewPanelObjectBase
        where TEdit : EditDocumentObjectBase
    {
        public IViewObjects<TView> ObjectSet { get; private set; }
        public IAddCommandHandler<TEdit> CmdAdd { get; private set; }
        public Commands.IEditCommandHandler<TEdit> CmdEdit { get; private set; }
        public Commands.IDeleteCommandHandler<TEdit> CmdDelete { get; private set; }

        public ListPanelViewModelBaseParamSet(
            IViewObjects<TView> objectSet,
            IAddCommandHandler<TEdit> cmdAdd,
            Commands.IEditCommandHandler<TEdit> cmdEdit,
            Commands.IDeleteCommandHandler<TEdit> cmdDelete
            )
        {
            Contract.Assert(EntityTypeDict.GetValueForObjectType<TView>() == EntityTypeDict.GetValueForObjectType<TEdit>());

            ObjectSet = objectSet;
            CmdAdd = cmdAdd;
            CmdEdit = cmdEdit;
            CmdDelete = cmdDelete;
        }
    }

    public abstract class ListPanelViewModelBase<TView, TEdit> : 
        ListPanelViewModelBase<TView>
        where TView: ViewPanelObjectBase
        where TEdit : EditDocumentObjectBase
    {
        public ListPanelViewModelBase(
            ListPanelViewModelBaseParamSet<TView, TEdit> parameters
            )
            : base(parameters.ObjectSet)
        {
            //TODO - przerobić na opóźnione tworzenie...
            Commands.Add(_CreateCommand(
                CmdKey.Add,
                parameters.CmdAdd.Execute, parameters.CmdAdd.CanExecute));
            Commands.Add(CreateCommandForSelected(
                CmdKey.Edit,
                parameters.CmdEdit));
            Commands.Add(CreateCommandForSelected(
                CmdKey.Delete,
                parameters.CmdDelete));
        }
    }

    public class CarsListPanelViewModel : ListPanelViewModelBase<ObjectsViewPanels.Car, ObjectsEditDocuments.Car>
    {
        public override PanelIdentifier Identifier { get { return PanelIdentifier.Panel1; } }

        public CarsListPanelViewModel(
            ListPanelViewModelBaseParamSet<ObjectsViewPanels.Car, ObjectsEditDocuments.Car> parameters
            ) : base(parameters) { }
    }

    public class AddressesListPanelViewModel : ListPanelViewModelBase<ObjectsViewPanels.Address, ObjectsEditDocuments.Address>
    {
        public override PanelIdentifier Identifier { get { return PanelIdentifier.Panel1; } }

        public AddressesListPanelViewModel(
            ListPanelViewModelBaseParamSet<ObjectsViewPanels.Address, ObjectsEditDocuments.Address> parameters
            ) : base(parameters) { }
    }

    public class PersonsListPanelViewModel : ListPanelViewModelBase<ObjectsViewPanels.Person, ObjectsEditDocuments.Person>
    {
        public override PanelIdentifier Identifier { get { return PanelIdentifier.Panel1; } }

        public PersonsListPanelViewModel(
            ListPanelViewModelBaseParamSet<ObjectsViewPanels.Person, ObjectsEditDocuments.Person> parameters
            ) : base(parameters) { }
    }

    public class PeriodsListPanelViewModel : ListPanelViewModelBase<ObjectsViewPanels.Period, ObjectsEditDocuments.Period>
    {
        public override PanelIdentifier Identifier { get { return PanelIdentifier.Panel2; } }

        public PeriodsListPanelViewModel(
            ListPanelViewModelBaseParamSet<ObjectsViewPanels.Period, ObjectsEditDocuments.Period> parameters
            ) : base(parameters) { }
    }

    public class RoutesListPanelViewModel : ListPanelViewModelBase<ObjectsViewPanels.Route, ObjectsEditDocuments.Route>
    {
        public override PanelIdentifier Identifier { get { return PanelIdentifier.Panel1; } }

        public RoutesListPanelViewModel(
            ListPanelViewModelBaseParamSet<ObjectsViewPanels.Route, ObjectsEditDocuments.Route> parameters
            ) : base(parameters) { }
    }
}
