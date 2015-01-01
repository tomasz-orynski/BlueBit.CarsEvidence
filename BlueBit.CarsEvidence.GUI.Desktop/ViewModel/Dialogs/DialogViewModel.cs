using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Windows.Input;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Dialogs
{
    public abstract class DialogViewModelBase : 
        ViewModelWithCloseBase,
        IObjectWithCommands
    {
        //TODO - przerobić na opóźnione tworzenie...
        private readonly CommandsViewModel _commands = new CommandsViewModel();
        public CommandsViewModel Commands { get { return _commands; } }
    }

    public interface IActionDialogViewModel<T>
        where T: DialogViewModelBase
    {
        Func<T, bool> CanExecute { get; set; }
        Func<T, bool> Execute { get; set; }
    }

    public abstract class ActionDialogViewModelBase<T> :
        DialogViewModelBase,
        IActionDialogViewModel<T>
        where T: DialogViewModelBase
    {
        protected abstract T This { get; }

        public Func<T, bool> CanExecute { get; set; }
        public Func<T, bool> Execute { get; set; }

        protected ActionDialogViewModelBase()
        {
            Commands.Add(_CreateCommand(CmdKey.Apply, OnExecute, OnCanExecute));
            Commands.Add(_CreateCommand(CmdKey.Cancel, OnClose));
        }

        private bool OnCanExecute()
        {
            Contract.Assert(CanExecute != null);
            return CanExecute(This);
        }
        private void OnExecute()
        {
            Contract.Assert(Execute != null);
            if (Execute(This))
                OnClose();
        }
    }
}
