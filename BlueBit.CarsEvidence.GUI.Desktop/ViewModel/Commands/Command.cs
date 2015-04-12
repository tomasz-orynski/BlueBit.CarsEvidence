using BlueBit.CarsEvidence.Commons.Templates;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Commands
{
    public interface ICommandHandler
    {
        bool CanExecute();
        void Execute();
    }

    public interface ICommandHandlerWithResult<TResult> :
        ICommandHandler
    {
        event Action<TResult> ExecuteResultHandle;
    }

    public interface ICommandHandlerForItem<T> :
        ICommandHandler
        where T: class
    {
        Func<T> Item { get; set; }
    }

    public abstract class CommandHandlerForItemBase<T, TResult> :
        ICommandHandlerForItem<T>,
        ICommandHandlerWithResult<TResult>
        where T : class
    {
        public event Action<TResult> ExecuteResultHandle;

        private Func<T> _Item;
        public Func<T> Item
        {
            get { return _Item; }
            set
            {
                Contract.Assert(_Item == null);
                Contract.Assert(value != null);
                _Item = value;
            }
        }

        public bool CanExecute()
        {
            Contract.Assert(Item != null);
            var item = Item();
            Contract.Assert(item != null);

            return OnCanExecute(item);
        }

        public void Execute()
        {
            Contract.Assert(Item != null);
            var item = Item();
            Contract.Assert(item != null);
            var result = OnExecute(item);
            if (ExecuteResultHandle != null)
                ExecuteResultHandle(result);
        }

        protected abstract bool OnCanExecute(T item);
        protected abstract TResult OnExecute(T item);
    }

    public abstract class CommandBase :
        ObservableObject,
        ICommand
    {
        public event EventHandler CanExecuteChanged;

        public virtual bool CanExecute(object parameter)
        {
            return true;
        }

        public abstract void Execute(object parameter);

        protected void Set<T>(
            ref T value, T newValue,
            Action onChange = null,
            [CallerMemberName] string propertyName = null
            )
        {
            Contract.Assert(!string.IsNullOrWhiteSpace(propertyName));
            if (Set<T>(propertyName, ref value, newValue))
            {
                if (onChange != null) onChange();
            }
        }
    }

    public class CommandsViewModel :
        IObjectWithItems<ObservableCollection<CommandInfo>>
    {
        private readonly Lazy<ObservableCollection<CommandInfo>> _items;

        public ObservableCollection<CommandInfo> Items { get { return _items.Value; } }

        public CommandInfo this[CmdKey key] { get { return _items.Value.Single(_ => _.Key == key); } }

        public CommandsViewModel()
        {
            _items = new Lazy<ObservableCollection<CommandInfo>>();
        }
        public CommandsViewModel(IEnumerable<CommandInfo> items)
        {
            _items = new Lazy<ObservableCollection<CommandInfo>>(
                () => new ObservableCollection<CommandInfo>(items));
        }

        public CommandInfo Add(CommandInfo cmd)
        {
            Contract.Assert(cmd != null);
            _items.Value.Add(cmd);
            return cmd;
        }
    }

    public interface IObjectWithCommands
    {
        CommandsViewModel Commands { get; }
    }

    public class CommandsGroupsViewModel :
        IObjectWithItems<ObservableCollection<CommandsGroupInfo>>
    {
        private readonly Lazy<ObservableCollection<CommandsGroupInfo>> _items;

        public ObservableCollection<CommandsGroupInfo> Items { get { return _items.Value; } }
        public long ItemsCount { get { return _items.Value.Count; } }

        public CommandsGroupsViewModel()
        {
            _items = new Lazy<ObservableCollection<CommandsGroupInfo>>();
        }
        public CommandsGroupsViewModel(IEnumerable<CommandsGroupInfo> items)
        {
            _items = new Lazy<ObservableCollection<CommandsGroupInfo>>(
                () => new ObservableCollection<CommandsGroupInfo>(items));
        }
    }
}
