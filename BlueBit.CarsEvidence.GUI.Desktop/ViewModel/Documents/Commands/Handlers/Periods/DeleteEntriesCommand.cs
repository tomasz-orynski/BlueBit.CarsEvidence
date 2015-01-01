using BlueBit.CarsEvidence.GUI.Desktop.Configuration.Attributes;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit.Documents;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Documents.Commands.Handlers.Periods
{
    public interface IDeleteEntriesCommandHandler :
        ICommandHandlerForSelected<PeriodEntry>
    {
    }

    [Register(typeof(IDeleteEntriesCommandHandler))]
    public class DeleteEntriesCommandHandler :
        CommandHandlerForSelectedBase<PeriodEntry>,
        IDeleteEntriesCommandHandler
    {
        protected override void OnExecute(IEnumerable<PeriodEntry> objects)
        {
            if (MessageBox.Show("Czy chcesz usunąć?", "TODO", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                objects.ToList().ForEach(_ => _.Period.RemoveFromEntries(_));
            }
        }

        protected override bool OnCanExecute(IEnumerable<PeriodEntry> objects)
        {
            return true;
        }
    }
}
