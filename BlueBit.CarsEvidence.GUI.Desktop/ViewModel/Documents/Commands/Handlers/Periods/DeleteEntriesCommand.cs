using BlueBit.CarsEvidence.GUI.Desktop.Configuration.Attributes;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit.Documents;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Documents.Commands.Handlers.Periods
{
    public interface IDeleteEntriesCommandHandler<TEntry> :
        ICommandHandlerForSelected<TEntry>
    {
    }

    public abstract class DeleteEntriesCommandHandlerBase<TEntry> :
        CommandHandlerForSelectedBase<TEntry>,
        IDeleteEntriesCommandHandler<TEntry>
    {
        protected abstract Action<TEntry> RemoveMethod { get; }

        protected sealed override void OnExecute(IEnumerable<TEntry> objects)
        {
            if (MessageBox.Show("Czy chcesz usunąć?", "TODO", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                objects.ToList().ForEach(RemoveMethod);
            }
        }

        protected sealed override bool OnCanExecute(IEnumerable<TEntry> objects)
        {
            return true;
        }
    }


    [Register(typeof(IDeleteEntriesCommandHandler<PeriodRouteEntry>))]
    public class DeleteRouteEntriesCommandHandler :
        DeleteEntriesCommandHandlerBase<PeriodRouteEntry>
    {
        private static void Remove(PeriodRouteEntry entry) { entry.Period.RemoveFromEntries(entry); }
        protected override Action<PeriodRouteEntry> RemoveMethod { get { return Remove; }}
    }

    [Register(typeof(IDeleteEntriesCommandHandler<PeriodFuelEntry>))]
    public class DeleteFuelEntriesCommandHandler :
        DeleteEntriesCommandHandlerBase<PeriodFuelEntry>
    {
        private static void Remove(PeriodFuelEntry entry) { entry.Period.RemoveFromEntries(entry); }
        protected override Action<PeriodFuelEntry> RemoveMethod { get { return Remove; } }
    }
}
