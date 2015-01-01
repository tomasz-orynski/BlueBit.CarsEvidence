using BlueBit.CarsEvidence.GUI.Desktop.Configuration.Attributes;
using BlueBit.CarsEvidence.GUI.Desktop.View.Dialogs;
using System;
using System.Diagnostics.Contracts;
using System.Windows;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel
{
    public interface IDialogService
    {
        bool? Show<TViewModel, TDialog>(TViewModel viewModel)
            where TViewModel : Dialogs.DialogViewModelBase
            where TDialog : Window, new();

        bool? Show<TViewModel>(TViewModel viewModel) 
            where TViewModel : Dialogs.DialogViewModelBase;
    }

    [Register(typeof(IDialogService))]
    public class DialogService :
        IDialogService
    {
        public bool? Show<TViewModel, TView>(TViewModel viewModel)
            where TViewModel : Dialogs.DialogViewModelBase
            where TView: Window, new()
        {
            Contract.Assert(viewModel != null);

            var view = new TView();
            view.Owner = App.Current.MainWindow;
            view.DataContext = viewModel;
            return view.ShowDialog();
        }

        public bool? Show<TViewModel>(TViewModel viewModel)
            where TViewModel : Dialogs.DialogViewModelBase
        {
            return Show<TViewModel, DialogView>(viewModel);
        }
    }

    public interface IDialogService<TViewModel>
        where TViewModel : Dialogs.DialogViewModelBase
    {
        bool? Show(
            Action<TViewModel> initializeAction,
            Func<TViewModel, bool> canExecuteAction,
            Func<TViewModel, bool> executeAction
            );
    }

    public class DialogService<TViewModel> :
        IDialogService<TViewModel>
        where TViewModel : Dialogs.DialogViewModelBase, Dialogs.IActionDialogViewModel<TViewModel>
    {
        private readonly Func<TViewModel> _viewModelCreator;

        public DialogService(
            Func<TViewModel> viewModelCreator)
        {
            _viewModelCreator = viewModelCreator;
        }

        public bool? Show(
            Action<TViewModel> initializeAction,
            Func<TViewModel, bool> canExecuteAction,
            Func<TViewModel, bool> executeAction
            )
        {
            Contract.Assert(executeAction != null);
            Contract.Assert(canExecuteAction != null);
            Contract.Assert(initializeAction != null);

            var viewModel = _viewModelCreator();
            var view = new DialogView();

            initializeAction(viewModel);
            viewModel.Execute = executeAction;
            viewModel.CanExecute = canExecuteAction;

            //TODO viewModel.CloseRequest += (s, e) => view.Close();

            view.Owner = App.Current.MainWindow;
            view.DataContext = viewModel;
            return view.ShowDialog();
        }
    }
}
