using BlueBit.CarsEvidence.GUI.Desktop.Model;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Commands;
using System.Diagnostics.Contracts;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Documents.Commands.Handlers
{
    public interface IApplyCommandHandler<T> :
        ICommandHandlerForItem<T>,
        ICommandHandlerWithResult<bool>
        where T : EditObjectBase
    {
    }

    public class ApplyCommandHandler<T> :
        CommandHandlerForItemBase<T, bool>,
        IApplyCommandHandler<T>
        where T : EditObjectBase
    {
        private readonly IEditObjects<T> _editObjects;

        public ApplyCommandHandler(IEditObjects<T> editObjects)
        {
            Contract.Assert(editObjects != null);
            _editObjects = editObjects;
        }

        protected override bool OnExecute(T item)
        {
            _editObjects.Save(item);
            return true;
        }

        protected override bool OnCanExecute(T item)
        {
            return !item.HasErrors;
        }
    }
}
