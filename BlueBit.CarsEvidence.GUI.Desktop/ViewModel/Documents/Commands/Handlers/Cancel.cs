using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Commands;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Documents.Commands.Handlers
{
    public interface ICancelCommandHandler<T> :
        ICommandHandlerForItem<T>,
        ICommandHandlerWithResult<bool>
        where T : EditObjectBase
    {
    }

    public class CancelCommandHandler<T> :
        CommandHandlerForItemBase<T, bool>,
        ICancelCommandHandler<T>
        where T : EditObjectBase
    {
        protected override bool OnExecute(T item)
        {
            //TODO - wykrywanie zmian i wtedy pytanie ???
            return true;
        }

        protected override bool OnCanExecute(T item)
        {
            return true;
        }
    }
}
