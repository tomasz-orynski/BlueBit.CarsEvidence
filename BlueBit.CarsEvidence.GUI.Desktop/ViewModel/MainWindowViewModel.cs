using BlueBit.CarsEvidence.Commons.Templates;
using BlueBit.CarsEvidence.GUI.Desktop.Model;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.View.General;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Commands;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Commands.Handlers;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Panels.Commands;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Panels.Commands.Handlers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel
{
    public sealed partial class MainWindowViewModel : 
        ViewModelWithCloseBase,
        IObjectWithItem<Company>,
        IObjectWithCommands
    {
        //TODO - przerobiæ na opóŸnione tworzenie...
        private readonly CommandsViewModel _commands = new CommandsViewModel();
        public CommandsViewModel Commands { get { return _commands; } }

        private readonly Lazy<CommandsGroupsViewModel> _repositoryCommandsGroups;
        public CommandsGroupsViewModel RepositoryCommandsGroups { get { return _repositoryCommandsGroups.Value; } }
        private readonly Lazy<CommandsGroupsViewModel> _repositoryExtraCommandsGroups;
        public CommandsGroupsViewModel RepositoryExtraCommandsGroups { get { return _repositoryExtraCommandsGroups.Value; } }

        private readonly ObservableCollection<Panels.PanelViewModelBase> _panelViewModels = new ObservableCollection<Panels.PanelViewModelBase>();
        public ObservableCollection<Panels.PanelViewModelBase> PanelViewModels { get { return _panelViewModels; } }

        private readonly ObservableCollection<Documents.DocumentViewModelBase> _documentViewModels = new ObservableCollection<Documents.DocumentViewModelBase>();
        public ObservableCollection<Documents.DocumentViewModelBase> DocumentViewModels { get { return _documentViewModels; } }

        public override sealed string Title { 
            get {
                var appNameVer = this.GetAppNameVersion();
                return string.Format("{0} {1}", appNameVer.Item1, appNameVer.Item2);
            } 
        }

        private Company _item;
        public Company Item { get { return _item; } set { Set(() => Item, ref _item, value); } }

        public MainWindowViewModel(
            IViewObjects<Company> objectSet,
            Func<IEnumerable<IShowCommandHandler>> showCommands,
            Func<IEnumerable<IAddCommandHandler>> addCommands,
            Func<IEnumerable<IEditAllCommandHandler>> editAllCommands,
            Func<IEnumerable<IDataCommandHandler>> dataCommands,
            Func<ISettingsCommandHandler> settingsCommand
            )
        {
            _item = objectSet.Items[0];
            _repositoryCommandsGroups = new Lazy<CommandsGroupsViewModel>(() => CreateRepositoryCommandsGroups(showCommands, addCommands, editAllCommands));
            _repositoryExtraCommandsGroups = new Lazy<CommandsGroupsViewModel>(() => CreateRepositoryExtraCommandsGroups(dataCommands));

            //TODO - przerobiæ na opóŸnione tworzenie...
            Commands.Add(CreateCommand(
                CmdKey.Settings, settingsCommand()));
            Commands.Add(_CreateCommand(
                CmdKey.Exit, CmdClose));
        }

        public Panels.PanelViewModelBase AddPanelViewModel(Panels.PanelViewModelBase viewModel)
        {
            _panelViewModels.Add(viewModel);
            return viewModel;
        }
        public Documents.DocumentViewModelBase AddDocumentViewModel(Documents.DocumentViewModelBase viewModel)
        {
            viewModel.Closed += RemoveDocumentViewModel;
            _documentViewModels.Add(viewModel);
            return viewModel;
        }
        private void RemoveDocumentViewModel(object sender, EventArgs args)
        {
            var viewModel = (Documents.DocumentViewModelBase)sender;
            _documentViewModels.Remove(viewModel);
            viewModel.Closed -= RemoveDocumentViewModel;
        }
    }
}