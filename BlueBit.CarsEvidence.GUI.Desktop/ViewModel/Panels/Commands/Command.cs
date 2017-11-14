using BlueBit.CarsEvidence.GUI.Desktop.Model;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit.Documents;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Documents;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Interactions;
using dotNetExt;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Panels.Commands
{
    public interface IOpenEditDocumentCommandHelper<T>
        where T : EditDocumentObjectBase
    {
        bool CanEditDocument(IEnumerable<ObjectWithIDBase> objects);
        void OpenEditDocument(IEnumerable<ObjectWithIDBase> objects);
        void NewEditDocument();
    }

    public class OpenEditDocumentCommandHelper<T> :
        IOpenEditDocumentCommandHelper<T>
        where T : EditDocumentObjectBase
    {
        private readonly IEditObjects<T> _editObjects;
        private readonly IEditDocumentViewModelCreator<EditDocumentViewModelBase<T>, T> _editDocumentViewModelCreator;

        public OpenEditDocumentCommandHelper(
            IEditObjects<T> editObjects,
            IEditDocumentViewModelCreator<EditDocumentViewModelBase<T>, T> editDocumentViewModelCreator
            )
        {
            Contract.Assert(editObjects != null);
            Contract.Assert(editDocumentViewModelCreator != null);

            _editObjects = editObjects;
            _editDocumentViewModelCreator = editDocumentViewModelCreator;
        }

        public bool CanEditDocument(IEnumerable<ObjectWithIDBase> objects)
        {
            Contract.Assert(objects != null);
            return true;
        }
        public void OpenEditDocument(IEnumerable<ObjectWithIDBase> objects)
        {
            Contract.Assert(objects != null);
            objects.Each(item => {
                var obj = _editObjects.Get(item.ID);
                var doc = _editDocumentViewModelCreator.GetInstanceAndActivate(obj) ?? _editDocumentViewModelCreator.Create(obj);
            });
        }
        public void NewEditDocument()
        {
            _editDocumentViewModelCreator.Create(_editObjects.Create());
        }
    }
}
