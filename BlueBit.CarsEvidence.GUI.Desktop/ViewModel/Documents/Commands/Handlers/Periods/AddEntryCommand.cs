using BlueBit.CarsEvidence.GUI.Desktop.Configuration.Attributes;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit.Documents;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Commands;
using System;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Documents.Commands.Handlers.Periods
{
    public interface IAddEntryCommandHandler<TEntry> :
        ICommandHandlerForItem<Period>
    {
    }

    [Register(typeof(IAddEntryCommandHandler<PeriodRouteEntry>))]
    public class AddRouteEntryCommandHandler :
        CommandHandlerForItemBase<Period, bool>,
        IAddEntryCommandHandler<PeriodRouteEntry>
    {
        private readonly Func<PeriodRouteEntry> _periodEntryCreator;

        public AddRouteEntryCommandHandler(
            Func<PeriodRouteEntry> periodEntryCreator)
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

    [Register(typeof(IAddEntryCommandHandler<PeriodFuelEntry>))]
    public class AddFuelEntryCommandHandler :
        CommandHandlerForItemBase<Period, bool>,
        IAddEntryCommandHandler<PeriodFuelEntry>
    {
        private readonly Func<PeriodFuelEntry> _periodEntryCreator;

        public AddFuelEntryCommandHandler(
            Func<PeriodFuelEntry> periodEntryCreator)
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
