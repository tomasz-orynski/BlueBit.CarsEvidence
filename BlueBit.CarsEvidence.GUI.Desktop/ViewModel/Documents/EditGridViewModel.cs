using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Documents
{
    public abstract class EditGridViewModelBase<TParent, TItem> :
        ViewModelBase
        where TParent: class
    {
        public override string Title { get { return string.Empty; } } //TODO
        public abstract ObservableCollection<TItem> Items { get; }

        private readonly Lazy<ISelectionCommand<TItem>> _cmdSelected = CreateCommandForSelectionLazy<TItem>();

        private readonly CommandsViewModel _itemsCommands = new CommandsViewModel();
        public CommandsViewModel ItemsCommands { get { return _itemsCommands; } }

        public ICommand EventCmdItemsSelectectionChanged { get { return _cmdSelected.Value; } }

        private IEnumerable<TItem> GetSelectedSet() { return _cmdSelected.Value.SelectedSet; }
        protected CommandInfo CreateCommandForSelected<TCmd>(CmdKey key, TCmd cmd)
            where TCmd : ICommandHandlerForSelected<TItem>
        {
            cmd.SelectedSet = GetSelectedSet;
            return CreateCommand(key, cmd);
        }
    }
}
