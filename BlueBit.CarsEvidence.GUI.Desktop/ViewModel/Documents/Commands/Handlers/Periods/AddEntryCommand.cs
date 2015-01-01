using BlueBit.CarsEvidence.GUI.Desktop.Configuration.Attributes;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit.Documents;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Commands;
using System;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Documents.Commands.Handlers.Periods
{
    public interface IAddEntryCommandHandler :
        ICommandHandlerForItem<Period>
    {
    }

    [Register(typeof(IAddEntryCommandHandler))]
    public class AddEntryCommandHandler :
        CommandHandlerForItemBase<Period, bool>,
        IAddEntryCommandHandler
    {
        private readonly Func<PeriodEntry> _periodEntryCreator;

        public AddEntryCommandHandler(
            Func<PeriodEntry> periodEntryCreator)
        {
            _periodEntryCreator = periodEntryCreator;
        }

        protected override bool OnCanExecute(Period item)
        {
            return true;
        }

        protected override bool OnExecute(Period item)
        {
            item.AddToEntries(_periodEntryCreator());
            return true;
        }
    }
}
