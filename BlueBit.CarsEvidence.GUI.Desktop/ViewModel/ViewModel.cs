using BlueBit.CarsEvidence.Commons.Reflection;
using BlueBit.CarsEvidence.Commons.Templates;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Mvvm = GalaSoft.MvvmLight;
using dotNetExt;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Commands;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using BlueBit.CarsEvidence.GUI.Desktop.Resources;
using System.Windows.Media;
using System.ComponentModel;
using BlueBit.CarsEvidence.BL.Alghoritms;
using System.Diagnostics;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.View.General;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel
{
    public interface IViewModel
    {
        ImageSource ImageSource { get; }
    }

    public interface IViewModelWithClose
    {
        event EventHandler<EventArgs> Closed;
        event EventHandler<EventArgs> Closed_Weak;

        ICommand EventCmdClosing { get; }
    }

    public interface IViewModelWithVisibility
    {
        event EventHandler<EventArgs> IsVisibleChanged;
        event EventHandler<EventArgs> IsVisibleChanged_Weak;

        bool IsVisible { get; set; }
    }

    public abstract class ViewModelBase : 
        Mvvm.ViewModelBase
    {
        public abstract string Title { get; }

        protected new bool Set<T>(ref T value, T newValue, [CallerMemberName] string propertyName = null)
        {
            Contract.Assert(!string.IsNullOrWhiteSpace(propertyName));
            return Set<T>(propertyName, ref value, newValue);
        }

        protected static ICommand _CreateCommand(Action execute, Func<bool> canExecute = null)
        {
            return new RelayCommand(execute, canExecute);
        }

        protected static CommandInfo _CreateCommand(CmdKey key, ICommand command, ICommandHandler commandHandler = null)
        {
            return new CommandInfo()
            {
                Key = key,
                Command = command,
                CommandHandler = commandHandler,
            };
        }

        protected static CommandInfo _CreateCommand(CmdKey key, Action execute, Func<bool> canExecute = null)
        {
            return _CreateCommand(key, _CreateCommand(execute, canExecute));
        }

        protected static CommandInfo CreateCommand<TCmd>(CmdKey key, TCmd cmd)
            where TCmd : ICommandHandler
        {
            return _CreateCommand(key, _CreateCommand(cmd.Execute, cmd.CanExecute), cmd);
        }

        protected static CommandInfo CreateCommandForItem<TCmd, T>(CmdKey key, TCmd cmd, Func<T> getItem)
            where TCmd: ICommandHandlerForItem<T>
            where T: class
        {
            cmd.Item = getItem;
            return _CreateCommand(key, _CreateCommand(cmd.Execute, cmd.CanExecute), cmd);
        }

        protected static Lazy<ISelectionCommand<T>> CreateCommandForSelectionLazy<T>()
        {
            return new Lazy<ISelectionCommand<T>>(() => new SelectionCommand<T>());
        }

        protected static Lazy<CommandsViewModel> CreateCommansViewModelLazy(Func<IEnumerable<CommandInfo>> prepareItems)
        {
            return new Lazy<CommandsViewModel>(() => new CommandsViewModel(prepareItems()));
        }
    }

    public abstract class ViewModelWithCloseBase :
        ViewModelBase,
        IViewModelWithClose
    {
        public event EventHandler<EventArgs> Closed;
        public event EventHandler<EventArgs> Closed_Weak
        {
            add { WeakEventManager<ViewModelWithCloseBase, EventArgs>.AddHandler(this, EventHelper.GetEventName(() => this.Closed), value); }
            remove { WeakEventManager<ViewModelWithCloseBase, EventArgs>.RemoveHandler(this, EventHelper.GetEventName(() => this.Closed), value); }
        }

        public virtual ICommand EventCmdClosing { get { return new RelayCommand<CancelEventArgs>(OnEventClosing); } }
        public virtual ICommand EventCmdClose { get { return new RelayCommand(OnEventClose); } }
        public virtual ICommand CmdClose { get { return new RelayCommand(OnClose); } }

        protected virtual void OnEventClosing(CancelEventArgs args)
        {
        }
        protected virtual void OnEventClose()
        {
        }
        protected virtual void OnClose()
        {
            SendCloseRequest();
        }
        protected void SendCloseRequest()
        {
            if (Closed != null)
                Closed(this, new EventArgs());
        }
    }

    public abstract class ViewModelWithVisibilityBase :
        ViewModelBase,
        IViewModelWithVisibility
    {
        public event EventHandler<EventArgs> IsVisibleChanged;
        public event EventHandler<EventArgs> IsVisibleChanged_Weak
        {
            add { WeakEventManager<ViewModelWithVisibilityBase, EventArgs>.AddHandler(this, EventHelper.GetEventName(() => this.IsVisibleChanged), value); }
            remove { WeakEventManager<ViewModelWithVisibilityBase, EventArgs>.RemoveHandler(this, EventHelper.GetEventName(() => this.IsVisibleChanged), value); }
        }

        private bool _isVisible = false;
        public bool IsVisible { get { return _isVisible; } set { if (Set(ref _isVisible, value)) SendIsVisibleChanged(); } }

        private void SendIsVisibleChanged()
        {
            if (IsVisibleChanged != null)
                IsVisibleChanged(this, new EventArgs());
        }
    }

    internal static class ViewModelExtensions
    {
        public static string GetTitleFormatKey<T>(this IObjectWithItems<ObservableCollection<T>> @this)
            where T : IObjectForEntityType
        {
            Contract.Assert(@this != null);
            return string.Format(App.ResourceDictionary.StrObjectsFrmt, typeof(T).Name);
        }

        public static string GetTitleFormat<T>(this IObjectWithItems<ObservableCollection<T>> @this)
            where T : IObjectForEntityType
        {
            Contract.Assert(@this != null);
            var key = @this.GetTitleFormatKey();
            return App.ResourceDictionary.GetResource<string>(key);
        }

        public static string GetTitle<T>(this IObjectWithItems<ObservableCollection<T>> @this)
            where T : IObjectForEntityType
        {
            Contract.Assert(@this != null);
            var frmt = @this.GetTitleFormat();
            return frmt;
        }

        public static string GetTitleFormatKey<T>(this IObjectWithItem<T> @this)
            where T : IObjectForEntityType
        {
            Contract.Assert(@this != null);
            return string.Format(App.ResourceDictionary.StrObjectFrmt, typeof(T).Name);
        }

        public static string GetTitleFormat<T>(this IObjectWithItem<T> @this)
            where T : IObjectForEntityType
        {
            Contract.Assert(@this != null);
            var key = @this.GetTitleFormatKey();
            return App.ResourceDictionary.GetResource<string>(key);
        }

        public static string GetTitle<T>(this IObjectWithItem<T> @this)
            where T : IObjectForEntityType, IObjectWithDescriptionForTitle
        {
            Contract.Assert(@this != null);
            Contract.Assert(@this.Item != null);
            var frmt = @this.GetTitleFormat();
            return string.Format(frmt, @this.Item.DescriptionForTitle);
        }

        public static Tuple<string, Version> GetAppNameVersion<T>(this T t)
        {
            var assembly = typeof(T).Assembly;
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);

            return Tuple.Create(fvi.ProductName, assembly.GetName().Version);
        }
    }
}
