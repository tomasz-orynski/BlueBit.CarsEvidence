using BlueBit.CarsEvidence.Commons.Diagnostics;
using BlueBit.CarsEvidence.GUI.Desktop.Model;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit.Documents;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Commands;
using System.Collections.Generic;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Panels.Commands
{
    public interface IEditAllCommandHandler :
        ICommandHandler,
        IObjectForEntityType
    {
    }

    public interface IEditAllCommandHandler<T> :
        IEditAllCommandHandler
        where T : EditDocumentObjectBase
    {
    }

    public class EditAllCommandHandler<T> :
        IEditAllCommandHandler<T>
        where T : EditDocumentObjectBase
    {
#if DEBUG
        private readonly object _dbgSc = new SingletonChecker<EditAllCommandHandler<T>>();
#endif
        private readonly IEditObjects<T> _editObjects;
        private readonly IOpenEditDocumentCommandHelper<T> _helper;

        public EntityType ForType { get { return EntityTypeDict.GetValueForObjectType<T>(); } }

        public EditAllCommandHandler(
            IEditObjects<T> editObjects,
            IOpenEditDocumentCommandHelper<T> helper
            )
        {
            _editObjects = editObjects;
            _helper = helper;
        }

        public void Execute()
        {
            _helper.OpenEditDocument(_editObjects.GetAll());
        }

        public bool CanExecute()
        {
            return _helper.CanEditDocument(_editObjects.GetAll());
        }
    }
}
