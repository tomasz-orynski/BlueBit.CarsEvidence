using BlueBit.CarsEvidence.Commons.Diagnostics;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit.Documents;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Commands;
using System.Collections.Generic;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Panels.Commands
{
    public interface IEditCommandHandler<T> :
        ICommandHandlerForSelected<ObjectBase>
        where T : EditDocumentObjectBase
    {
    }

    public class EditCommandHandler<T> :
        CommandHandlerForSelectedBase<ObjectBase>,
        IEditCommandHandler<T>
        where T : EditDocumentObjectBase
    {
#if DEBUG
        private readonly object _dbgSc = new SingletonChecker<EditCommandHandler<T>>();
#endif
        private readonly IOpenEditDocumentCommandHelper<T> _helper;

        public EditCommandHandler(
            IOpenEditDocumentCommandHelper<T> helper
            )
        {
            _helper = helper;
        }

        protected override void OnExecute(IEnumerable<ObjectBase> objects)
        {
            _helper.OpenEditDocument(objects);
        }

        protected override bool OnCanExecute(IEnumerable<ObjectBase> objects)
        {
            return _helper.CanEditDocument(objects);
        }
    }
}
