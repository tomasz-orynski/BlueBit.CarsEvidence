using BlueBit.CarsEvidence.Commons.Templates;
using BlueBit.CarsEvidence.GUI.Desktop.Model;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit.Documents;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Documents;
using dotNetExt;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Panels.Commands
{
    public interface IOpenEditDocumentCommandHelper<T>
        where T : EditDocumentObjectBase
    {
        bool CanEditDocument(IEnumerable<ObjectBase> objects);
        void OpenEditDocument(IEnumerable<ObjectBase> objects);
        void NewEditDocument();
    }

    public class OpenEditDocumentCommandHelper<T> :
        IOpenEditDocumentCommandHelper<T>
        where T : EditDocumentObjectBase
    {
        private readonly IEditObjects<T> _editObjects;
        private readonly ISingletonCreatorForItem<EditDocumentViewModelBase<T>, T> _editDocumentViewModelCreator;

        public OpenEditDocumentCommandHelper(
            IEditObjects<T> editObjects,
            ISingletonCreatorForItem<EditDocumentViewModelBase<T>, T> editDocumentViewModelCreator
            )
        {
            Contract.Assert(editObjects != null);
            Contract.Assert(editDocumentViewModelCreator != null);

            _editObjects = editObjects;
            _editDocumentViewModelCreator = editDocumentViewModelCreator;
        }

        public bool CanEditDocument(IEnumerable<ObjectBase> objects)
        {
            Contract.Assert(objects != null);
            return objects.All(item => _editDocumentViewModelCreator.GetInstance(_editObjects.Get(item.ID)) == null);
        }
        public void OpenEditDocument(IEnumerable<ObjectBase> objects)
        {
            Contract.Assert(objects != null);
            objects.Each(item => {
                var obj = _editObjects.Get(item.ID);
                var doc = _editDocumentViewModelCreator.GetInstance(obj) ?? _editDocumentViewModelCreator.Create(obj);
            });
        }
        public void NewEditDocument()
        {
            _editDocumentViewModelCreator.Create(_editObjects.Create());
        }
    }
}
