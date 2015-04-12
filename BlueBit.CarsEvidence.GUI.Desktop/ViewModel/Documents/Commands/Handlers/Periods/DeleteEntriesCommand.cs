using BlueBit.CarsEvidence.GUI.Desktop.Configuration.Attributes;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit.Documents;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Documents.Commands.Handlers.Periods
{
    public interface IDeleteEntriesCommandHandler<TEntry> :
        ICommandHandlerForSelected<TEntry>
    {
    }

    [Register(typeof(IDeleteEntriesCommandHandler<PeriodRouteEntry>))]
    public class DeleteRouteEntriesCommandHandler :
        CommandHandlerForSelectedBase<PeriodRouteEntry>,
        IDeleteEntriesCommandHandler<PeriodRouteEntry>
    {
        protected override void OnExecute(IEnumerable<PeriodRouteEntry> objects)
        {
            if (MessageBox.Show("Czy chcesz usunąć?", "TODO", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                objects.ToList().ForEach(_ => _.Period.RemoveFromEntries(_));
            }
        }

        protected override bool OnCanExecute(IEnumerable<PeriodRouteEntry> objects)
        {
            return true;
        }
    }

    [Register(typeof(IDeleteEntriesCommandHandler<PeriodFuelEntry>))]
    public class DeleteFuelEntriesCommandHandler :
        CommandHandlerForSelectedBase<PeriodFuelEntry>,
        IDeleteEntriesCommandHandler<PeriodFuelEntry>
    {
        protected override void OnExecute(IEnumerable<PeriodFuelEntry> objects)
        {
            if (MessageBox.Show("Czy chcesz usunąć?", "TODO", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                objects.ToList().ForEach(_ => _.Period.RemoveFromEntries(_));
            }
        }

        protected override bool OnCanExecute(IEnumerable<PeriodFuelEntry> objects)
        {
            return true;
        }
    }
}
