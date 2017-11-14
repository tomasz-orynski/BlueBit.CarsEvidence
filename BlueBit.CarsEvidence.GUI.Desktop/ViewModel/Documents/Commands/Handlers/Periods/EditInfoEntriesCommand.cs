using BlueBit.CarsEvidence.GUI.Desktop.Configuration.Attributes;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit.Documents;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Documents.Commands.Handlers.Periods
{
    public interface IEditInfoEntriesCommandHandler<TEntry> :
        ICommandHandlerForSelected<TEntry>
    {
    }

    public abstract class EditInfoEntriesCommandHandlerBase<TEntry> :
        CommandHandlerForSelectedBase<TEntry>,
        IEditInfoEntriesCommandHandler<TEntry>
        where TEntry : IEditObjectWithInfo
    {
        private readonly Func<IEditInfoCommand> _editInfoCmd;

        protected EditInfoEntriesCommandHandlerBase(
            Func<IEditInfoCommand> editInfoCmd
            )
        {
            _editInfoCmd = editInfoCmd;
        }

        protected sealed override void OnExecute(IEnumerable<TEntry> objects)
        {
            var cmd = _editInfoCmd();
            cmd.Item = () => objects.First();
            cmd.Execute();
        }

        protected sealed override bool OnCanExecute(IEnumerable<TEntry> objects)
        {
            if(objects.Take(2).Count() != 1)
                return false;

            var cmd = _editInfoCmd();
            cmd.Item = () => objects.First();
            return cmd.CanExecute();
        }
    }

    [Register(typeof(IEditInfoEntriesCommandHandler<PeriodRouteEntry>))]
    public class EditInfoRouteEntriesCommandHandler :
        EditInfoEntriesCommandHandlerBase<PeriodRouteEntry>
    {
        public EditInfoRouteEntriesCommandHandler(
            Func<IEditInfoCommand> editInfoCmd
            )
            : base(editInfoCmd)
        {
        }
    }

    [Register(typeof(IEditInfoEntriesCommandHandler<PeriodFuelEntry>))]
    public class EditInfoFuelEntriesCommandHandler :
        EditInfoEntriesCommandHandlerBase<PeriodFuelEntry>
    {
        public EditInfoFuelEntriesCommandHandler(
            Func<IEditInfoCommand> editInfoCmd
            )
            : base(editInfoCmd)
        {
        }
    }
}
