using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using dotNetExt;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.ComponentModel;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Commands
{
    public interface ICommandHandlerForSelected<T> :
        ICommandHandler
    {
        Func<IEnumerable<T>> SelectedSet { get; set; }
    }

    public abstract class CommandHandlerForSelectedBase<T> :
        ICommandHandlerForSelected<T>
    {
        private Func<IEnumerable<T>> _SelectedSet;
        public Func<IEnumerable<T>> SelectedSet 
        { 
            get { return _SelectedSet; }
            set
            {
                Contract.Assert(_SelectedSet == null);
                Contract.Assert(value != null);
                _SelectedSet = value;
            }
        }

        public void Execute()
        {
            Contract.Assert(SelectedSet != null);
            var objects = SelectedSet();
            Contract.Assert(objects != null);
            OnExecute(objects);
        }

        public bool CanExecute()
        {
            Contract.Assert(SelectedSet != null);
            var objects = SelectedSet();
            Contract.Assert(objects != null);

            if (objects.Any())
                return OnCanExecute(objects);
            return false;
        }

        protected abstract bool OnCanExecute(IEnumerable<T> objects);
        protected abstract void OnExecute(IEnumerable<T> objects);
    }

    public interface ISelectionInfo :
        INotifyPropertyChanged
    {
        bool? SelectedState { get; set; }
        long SelectedCount { get; }
    }

    public interface ISelectionCommand<T> :
        ICommand,
        ISelectionInfo
    {
        IEnumerable<T> SelectedSet { get; }
    }

    public class SelectionCommand<T> :
        CommandBase,
        ISelectionCommand<T>
    {
        private interface _IItemsControl
        {
            Control Source { get; }

            void Clear();
            void Select();
            IEnumerable<T> GetSelected();
            long GetCount();
            bool? GetState();
        };

        private class _ListBox : _IItemsControl
        {
            private readonly ListBox _source;

            public _ListBox(ListBox source) { _source = source; }
            
            public Control Source { get { return _source; } }

            public void Clear() 
            { 
                _source.SelectedItems.Clear(); 
            }
            public void Select() 
            { 
                _source
                    .ItemsSource
                    .Each(_ => _source.SelectedItems.Add(_)); 
            }
            public IEnumerable<T> GetSelected() { return _source.SelectedItems.Cast<T>(); }
            public long GetCount() { return _source.SelectedItems.Count; }
            public bool? GetState()
            {
                var selCnt = _source.SelectedItems.Count;
                if (selCnt == 0) return false;

                var src = (ObservableCollection<T>)_source.ItemsSource;
                var srcCnt = src.Count;
                if (srcCnt == selCnt) return true;

                return null;
            }
        }
        private class _DataGrid : _IItemsControl
        {
            private readonly DataGrid _source;

            public _DataGrid(DataGrid source) { _source = source; }

            public Control Source { get { return _source; } }

            public void Clear() 
            { 
                _source.SelectedItems.Clear(); 
            }
            public void Select() 
            { 
                _source
                    .ItemsSource
                    .Each(_ => _source.SelectedItems.Add(_)); 
            }
            public IEnumerable<T> GetSelected() { return _source.SelectedItems.Cast<T>(); }
            public long GetCount() { return _source.SelectedItems.Count; }
            public bool? GetState()
            {
                var selCnt = _source.SelectedItems.Count;
                if (selCnt == 0) return false;

                var src = (ObservableCollection<T>)_source.ItemsSource;
                var srcCnt = src.Count;
                if (srcCnt == selCnt) return true;

                return null;
            }
        }

        private _IItemsControl _control;

        public IEnumerable<T> SelectedSet { get { return _control == null ? Enumerable.Empty<T>() : _control.GetSelected(); } }
        public long SelectedCount { get { return _control == null ? 0 : _control.GetCount(); } }

        public bool? SelectedState { 
            get
            {
                return _control == null ? false : _control.GetState();
            }
            set
            {
                switch (value)
                {
                    case true:
                        _control.Select();
                        break;

                    case false:
                        _control.Clear();
                        break;
                }
                SetSelected();
            }
        }

        public virtual bool CanExecute(object parameter)
        {
            RaisePropertyChanged(() => SelectedCount);
            return true;
        }

        public override void Execute(object parameter)
        {
            Contract.Assert(parameter != null);
            Contract.Assert(parameter is RoutedEventArgs);

            var routedEventArgs = (RoutedEventArgs)parameter;
            Contract.Assert(routedEventArgs.Source is Selector);
            if (routedEventArgs.RoutedEvent.Name == "Loaded")
            {
                if (routedEventArgs.Source is ListBox)
                {
                    _control = new _ListBox((ListBox)routedEventArgs.Source);
                    return;
                }
                if (routedEventArgs.Source is DataGrid)
                {
                    _control = new _DataGrid((DataGrid)routedEventArgs.Source);
                    return;
                }
            }

            Contract.Assert(_control != null);
            Contract.Assert(routedEventArgs.Source == _control.Source);
            Contract.Assert(routedEventArgs is SelectionChangedEventArgs);
            SetSelected();
        }

        private void SetSelected()
        {
            RaisePropertyChanged(() => SelectedState);
            RaisePropertyChanged(() => SelectedCount);
        }
    }
}
