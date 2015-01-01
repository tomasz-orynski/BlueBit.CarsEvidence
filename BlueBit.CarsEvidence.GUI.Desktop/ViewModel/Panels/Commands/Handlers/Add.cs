using BlueBit.CarsEvidence.Commons.Diagnostics;
using BlueBit.CarsEvidence.Commons.Templates;
using BlueBit.CarsEvidence.GUI.Desktop.Model;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Commands;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Documents;
using System;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Panels.Commands.Handlers
{
    public interface IAddCommandHandler :
        ICommandHandler,
        IObjectForEntityType
    {
    }

    public interface IAddCommandHandler<T> :
        IAddCommandHandler
        where T : EditObjectBase
    {
    }

    public class AddCommandHandler<T> :
        IAddCommandHandler<T>
        where T : EditObjectBase
    {
#if DEBUG
        private readonly object _dbgSc = new SingletonChecker<AddCommandHandler<T>>();
#endif
        private readonly IOpenEditDocumentCommandHelper<T> _helper;

        public EntityType ForType { get { return EntityTypeDict.GetValueForObjectType<T>(); } }

        public AddCommandHandler(
            IOpenEditDocumentCommandHelper<T> helper
            )
        {
            _helper = helper;
        }

        public void Execute()
        {
            _helper.NewEditDocument();
        }

        public bool CanExecute()
        {
            return true;
        }
    }
}
