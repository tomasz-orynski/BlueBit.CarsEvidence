using BlueBit.CarsEvidence.Commons.Linq;
using BlueBit.CarsEvidence.Commons.Templates;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit.Documents;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.View.Panels;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Documents;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Panels;
using System;
using System.Linq;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Interactions
{
    public interface IListPanelViewModelCreator<T> :
        ISingletonCreator<T>
        where T : ListPanelViewModelBase
    {
    }

    public class ListPanelViewModelCreator<T, TItem> :
        IListPanelViewModelCreator<T>
        where T : ListPanelViewModelBase<TItem>
        where TItem : ViewPanelObjectBase
    {
        private readonly Func<MainWindowViewModel> _model;
        private readonly Func<T> _creator;

        public ListPanelViewModelCreator(
            Func<MainWindowViewModel> model,
            Func<T> creator)
        {
            _model = model;
            _creator = creator;
        }

        public T Create()
        {
            var panel = _creator();
            var model = _model();
            model.AddPanelViewModel(panel);
            model.ActiveViewModel = panel;
            return panel;
        }

        public T GetInstance()
        {
            return _model().PanelViewModels.Castable<T>().FirstOrDefault();
        }
    }

    public interface IEditDocumentViewModelCreator<out T, in TItem> :
        ISingletonCreatorForItem<T, TItem>
        where T : EditDocumentViewModelBase<TItem>
        where TItem : EditDocumentObjectBase
    {
        T GetInstanceAndActivate(TItem item);
    }

    public class EditDocumentViewModelCreator<T, TItem> :
        IEditDocumentViewModelCreator<T, TItem>
        where T : EditDocumentViewModelBase<TItem>
        where TItem : EditDocumentObjectBase
    {
        private readonly Func<MainWindowViewModel> _model;
        private readonly Func<T> _creator;

        public EditDocumentViewModelCreator(
            Func<MainWindowViewModel> model,
            Func<T> creator)
        {
            _model = model;
            _creator = creator;
        }

        public T Create(TItem item)
        {
            var document = _creator();
            document.Item = item;
            var model = _model();
            model.AddDocumentViewModel(document);
            model.ActiveViewModel = document;
            item.Validate();
            return document;
        }

        public T GetInstance(TItem item)
        {
            return _model().DocumentViewModels.Castable<T>().FirstOrDefault(d => d.Item.ID == item.ID);
        }
        public T GetInstance(long itemID)
        {
            return _model().DocumentViewModels.Castable<T>().FirstOrDefault(d => d.Item.ID == itemID);
        }

        public T GetInstanceAndActivate(TItem item)
        {
            var model = _model();
            var instance = model.DocumentViewModels.Castable<T>().FirstOrDefault(d => d.Item.ID == item.ID);
            if (instance != null)
                model.ActiveViewModel = instance;
            return instance;
        }
    }
}
