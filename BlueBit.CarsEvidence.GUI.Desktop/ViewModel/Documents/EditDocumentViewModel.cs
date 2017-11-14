using BlueBit.CarsEvidence.Commons.Templates;
using BlueBit.CarsEvidence.GUI.Desktop.Configuration.Attributes;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit.Documents;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Commands;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Documents.Commands.Handlers;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Documents.Commands.Handlers.Periods;
using dotNetExt;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
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
        where T : ObjectWithIDBase
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

    public abstract class PeriodEntriesEditGridViewModelBase<TEntry> :
        EditGridViewModelBase<Period, TEntry>
    {
        public PeriodEditDocumentViewModel ParentModel { get; set; }
        protected Period GetParent() 
        {
            Contract.Assert(ParentModel != null);
            return ParentModel.Item;
        }
    }

    [Register(typeof(PeriodRouteEntriesEditGridViewModel))]
    public class PeriodRouteEntriesEditGridViewModel :
        PeriodEntriesEditGridViewModelBase<PeriodRouteEntry>
    {
        public override ObservableCollection<PeriodRouteEntry> Items { get { return GetParent().RouteEntries; } }

        public PeriodRouteEntriesEditGridViewModel(
            IAddEntryCommandHandler<PeriodRouteEntry> cmdAddEntry,
            IDeleteEntriesCommandHandler<PeriodRouteEntry> cmdDeleteEntries,
            IGenerateEntriesCommand cmdGenerateEntries,
            IEditInfoEntriesCommandHandler<PeriodRouteEntry> cmdEditInfo
            )
        {
            ItemsCommands.Add(CreateCommandForSelected(CmdKey.EditInfo,
                cmdEditInfo));
            ItemsCommands.Add(CreateCommandForItem(CmdKey.Generate,
                cmdGenerateEntries, GetParent));
            ItemsCommands.Add(CreateCommandForItem(CmdKey.Add,
                cmdAddEntry, GetParent));
            ItemsCommands.Add(CreateCommandForSelected(CmdKey.Delete,
                cmdDeleteEntries));
        }
    }

    [Register(typeof(PeriodFuelEntriesEditGridViewModel))]
    public class PeriodFuelEntriesEditGridViewModel :
        PeriodEntriesEditGridViewModelBase<PeriodFuelEntry>
    {
        public override ObservableCollection<PeriodFuelEntry> Items { get { return GetParent().FuelEntries; } }

        public PeriodFuelEntriesEditGridViewModel(
            Func<Period> parent,
            IAddEntryCommandHandler<PeriodFuelEntry> cmdAddEntry,
            IDeleteEntriesCommandHandler<PeriodFuelEntry> cmdDeleteEntries,
            IEditInfoEntriesCommandHandler<PeriodFuelEntry> cmdEditInfo
            )
        {
            ItemsCommands.Add(CreateCommandForSelected(CmdKey.EditInfo,
                cmdEditInfo));
            ItemsCommands.Add(CreateCommandForItem(CmdKey.Add,
                cmdAddEntry, GetParent));
            ItemsCommands.Add(CreateCommandForSelected(CmdKey.Delete,
                cmdDeleteEntries));
        }
    }

    public class PeriodEditDocumentViewModel : 
        EditDocumentViewModelBase<Period>
    {
        private readonly PeriodRouteEntriesEditGridViewModel _RouteEntries;
        public PeriodRouteEntriesEditGridViewModel RouteEntries { get { return _RouteEntries; } }
        private readonly PeriodFuelEntriesEditGridViewModel _FuelEntries;
        public PeriodFuelEntriesEditGridViewModel FuelEntries { get { return _FuelEntries; } }

        public PeriodEditDocumentViewModel(
            EditDocumentViewModelBaseParamSet<Period> parameters,
            PeriodRouteEntriesEditGridViewModel routeEntriesModel,
            PeriodFuelEntriesEditGridViewModel fuelEntriesModel
            )
            : base(parameters)
        {
            _RouteEntries = routeEntriesModel;
            _RouteEntries.ParentModel = this;
            _FuelEntries = fuelEntriesModel;
            _FuelEntries.ParentModel = this;
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
