using BlueBit.CarsEvidence.Commons.Diagnostics;
using BlueBit.CarsEvidence.Commons.Templates;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.View;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Commands;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Panels;
using System;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Panels.Commands.Handlers
{
    public interface IShowCommandHandler :
        ICommandHandler,
        IObjectForEntityType
    {
    }

    public interface IShowCommandHandler<T> :
        IShowCommandHandler
        where T : ViewObjectBase
    {
    }

    public class ShowCommandHandler<T> :
        IShowCommandHandler<T>
        where T : ViewObjectBase
    {
#if DEBUG
        private readonly object _dbgSc = new SingletonChecker<ShowCommandHandler<T>>();
#endif

        private readonly ISingletonCreator<ListPanelViewModelBase<T>> _listPanelViewModelCreator;

        public EntityType ForType { get { return EntityTypeDict.GetValueForObjectType<T>(); } }

        public ShowCommandHandler(
            ISingletonCreator<ListPanelViewModelBase<T>> listPanelViewModelCreator
            )
        {
            _listPanelViewModelCreator = listPanelViewModelCreator;
        }

        public bool CanExecute()
        {
            var instance = _listPanelViewModelCreator.GetInstance();
            return instance == null || !instance.IsVisible;
        }

        public void Execute()
        {
            var instance = _listPanelViewModelCreator.GetInstance() ?? _listPanelViewModelCreator.Create();
            instance.IsVisible = true;
        }
    }
}
