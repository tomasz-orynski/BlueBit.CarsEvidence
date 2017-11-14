using BlueBit.CarsEvidence.GUI.Desktop.Configuration.Attributes;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit.Documents;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Commands;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Dialogs.Commands;
using System;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Documents.Commands.Handlers
{
    public interface IEditInfoCommand :
        ICommandHandlerForItem<IEditObjectWithInfo>
    {
    }

    [Register(typeof(IEditInfoCommand))]
    public class EditInfoCommand :
        CommandHandlerForItemBase<IEditObjectWithInfo, bool?>,
        IEditInfoCommand
    {
        private readonly Func<IDialogService<EditInfoDialogViewModel>> _dialogService;
        private readonly Func<PeriodRouteEntry> _periodEntryCreator;

        public EditInfoCommand(
            Func<IDialogService<EditInfoDialogViewModel>> dialogService,
            Func<PeriodRouteEntry> periodEntryCreator)
        {
            _dialogService = dialogService;
            _periodEntryCreator = periodEntryCreator;
        }

        protected override bool OnCanExecute(IEditObjectWithInfo item)
        {
            return true;
        }

        protected override bool? OnExecute(IEditObjectWithInfo item)
        {
            return _dialogService().Show(
                dvm =>
                {
                    dvm.Item = item;
                    dvm.Info = item.Info;
                },
                _CanExecute,
                _Execute
            );
        }

        private bool _CanExecute(EditInfoDialogViewModel dvm)
        {
            return true;
        }
        private bool _Execute(EditInfoDialogViewModel dvm)
        {
            dvm.Item.Info = dvm.Info;
            return true;
        }
    }
}
