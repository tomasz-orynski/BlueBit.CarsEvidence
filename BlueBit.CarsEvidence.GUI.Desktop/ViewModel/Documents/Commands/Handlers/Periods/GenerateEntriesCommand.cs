using BlueBit.CarsEvidence.BL.Alghoritms.Validation;
using BlueBit.CarsEvidence.GUI.Desktop.Configuration.Attributes;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit.Documents;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Commands;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Dialogs.Commands.Periods;
using dotNetExt;
using System;
using System.Linq;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Documents.Commands.Handlers.Periods
{
    public interface IGenerateEntriesCommand :
        ICommandHandlerForItem<Period>
    {
    }

    [Register(typeof(IGenerateEntriesCommand))]
    public class GenerateEntriesCommand :
        CommandHandlerForItemBase<Period, bool?>,
        IGenerateEntriesCommand
    {
        private readonly Func<IDialogService<GenerateEntriesDialogViewModel>> _dialogService;
        private readonly Func<PeriodEntry> _periodEntryCreator;

        public GenerateEntriesCommand(
            Func<IDialogService<GenerateEntriesDialogViewModel>> dialogService,
            Func<PeriodEntry> periodEntryCreator)
        {
            _dialogService = dialogService;
            _periodEntryCreator = periodEntryCreator;
        }

        protected override bool OnCanExecute(Period item)
        {
            return item.Year.IsYearValid()
                && item.Month != null;
        }

        protected override bool? OnExecute(Period item)
        {
            return _dialogService().Show(
                dvm => {
                    dvm.Period = item;
                },
                _CanExecute,
                _Execute
            );
        }

        private bool _CanExecute(GenerateEntriesDialogViewModel dvm)
        {
            return dvm.Route != null && dvm.Person != null;
        }
        private bool _Execute(GenerateEntriesDialogViewModel dvm)
        {
            var period = dvm.Period;
            period.Entries.Clear();

            Func<int, bool> filter = (_) => {
                if (!dvm.OnlyWorkDays) return true;

                var dt = new DateTime(period.Year, period.Month.Number, _);
                return dt.DayOfWeek != DayOfWeek.Sunday && dt.DayOfWeek != DayOfWeek.Saturday;
            };

            Enumerable.Range(1, DateTime.DaysInMonth(period.Year, period.Month.Number))
                .Where(filter)
                .Each(day => {
                    var periodEntry = period.AddToEntries(_periodEntryCreator());
                    periodEntry.Day = periodEntry.AllDays.Single(_ => _.Number == day);
                    periodEntry.Person = dvm.Person;
                    periodEntry.Route = dvm.Route;
                });

            return true;
        }
    }
}
