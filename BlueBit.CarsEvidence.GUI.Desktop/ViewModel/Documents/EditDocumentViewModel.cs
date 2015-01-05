using BlueBit.CarsEvidence.BL.Alghoritms;
using BlueBit.CarsEvidence.Commons.Templates;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit.Documents;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Commands;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Documents.Commands.Handlers;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Documents.Commands.Handlers.Periods;
using dotNetExt;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Documents
{
    public abstract class EditDocumentViewModelBase : 
        DocumentViewModelBase,
        IObjectWithCommands
    {
        //TODO - przerobić na opóźnione tworzenie...
        private readonly CommandsViewModel _commands = new CommandsViewModel();
        public CommandsViewModel Commands { get { return _commands; } }
    }

    public interface IEditDocumentViewModel<T> :
        IObjectWithItem<T>,
        IObjectForEntityType
        where T : ObjectBase
    {
    }

    public class EditDocumentViewModelBaseParamSet<T>
        where T : EditObjectBase
    {
        public IApplyCommandHandler<T> CmdApply { get; private set; }
        public ICancelCommandHandler<T> CmdCancel { get; private set; }

        public EditDocumentViewModelBaseParamSet(
            IApplyCommandHandler<T> cmdApply,
            ICancelCommandHandler<T> cmdCancel
            )
        {
            CmdApply = cmdApply;
            CmdCancel = cmdCancel;
        }
    }

    public abstract class EditDocumentViewModelBase<T> : 
        EditDocumentViewModelBase,
        IEditDocumentViewModel<T>
        where T : EditDocumentObjectBase
    {
        public override sealed ICommand CmdClose { get { return Commands[CmdKey.Cancel].Command; } }

        public override sealed string Title { get { return this.GetTitle(); } }

        private T _item;
        public T Item { get { return _item; } set { Set(ref _item, value); } }

        public EntityType ForType { get { return EntityTypeDict.GetValueForObjectType<T>(); } }

        protected EditDocumentViewModelBase(
            EditDocumentViewModelBaseParamSet<T> parameters
            )
        {
            parameters.CmdApply.ExecuteResultHandle += (r) => r.IfTrue(SendCloseRequest);
            parameters.CmdCancel.ExecuteResultHandle += (r) => r.IfTrue(SendCloseRequest);

            //TODO - przerobić na opóźnione tworzenie...
            Commands.Add(CreateCommandForItem(
                CmdKey.Apply,
                parameters.CmdApply, () => Item));
            Commands.Add(CreateCommandForItem(
                CmdKey.Cancel,
                parameters.CmdCancel, () => Item));
        }

    }

    public class AddressEditDocumentViewModel : EditDocumentViewModelBase<Address>
    {
        public AddressEditDocumentViewModel(
            EditDocumentViewModelBaseParamSet<Address> parameters
            )
            : base(parameters)
        { }
    }

    public class CarEditDocumentViewModel : EditDocumentViewModelBase<Car>
    {
        public CarEditDocumentViewModel(
            EditDocumentViewModelBaseParamSet<Car> parameters
            )
            : base(parameters)
        { }
    }

    public class CompanyEditDocumentViewModel : EditDocumentViewModelBase<Company>
    {
        public CompanyEditDocumentViewModel(
            EditDocumentViewModelBaseParamSet<Company> parameters
            )
            : base(parameters)
        { }
    }

    public class PeriodEditDocumentViewModel : EditDocumentViewModelBase<Period>
    {
        private readonly Lazy<ISelectionCommand<PeriodEntry>> _cmdSelected = CreateCommandForSelectionLazy<PeriodEntry>();

        private readonly CommandsViewModel _entriesCommands = new CommandsViewModel();
        public CommandsViewModel EntriesCommands { get { return _entriesCommands; } }

        public ICommand EventCmdEntriesSelectectionChanged { get { return _cmdSelected.Value; } }

        public PeriodEditDocumentViewModel(
            EditDocumentViewModelBaseParamSet<Period> parameters,
            IAddEntryCommandHandler cmdAddEntry,
            IDeleteEntriesCommandHandler cmdDeleteEntries,
            IGenerateEntriesCommand cmdGenerateEntries
            )
            : base(parameters)
        {
            EntriesCommands.Add(CreateCommandForItem(CmdKey.Generate,
                cmdGenerateEntries, () => Item));
            EntriesCommands.Add(CreateCommandForItem(CmdKey.Add,
                cmdAddEntry, () => Item));
            EntriesCommands.Add(CreateCommandForSelected(CmdKey.Delete,
                cmdDeleteEntries));
        }

        private IEnumerable<PeriodEntry> GetSelectedSet() { return _cmdSelected.Value.SelectedSet; }
        protected CommandInfo CreateCommandForSelected<TCmd>(CmdKey key, TCmd cmd)
            where TCmd : ICommandHandlerForSelected<PeriodEntry>
        {
            cmd.SelectedSet = GetSelectedSet;
            return CreateCommand(key, cmd);
        }
    }

    public class PersonEditDocumentViewModel : EditDocumentViewModelBase<Person>
    {
        public PersonEditDocumentViewModel(
            EditDocumentViewModelBaseParamSet<Person> parameters
            )
            : base(parameters)
        { }
    }

    public class RouteEditDocumentViewModel : EditDocumentViewModelBase<Route>
    {
        public RouteEditDocumentViewModel(
            EditDocumentViewModelBaseParamSet<Route> parameters
            )
            : base(parameters)
        { }
    }
}
