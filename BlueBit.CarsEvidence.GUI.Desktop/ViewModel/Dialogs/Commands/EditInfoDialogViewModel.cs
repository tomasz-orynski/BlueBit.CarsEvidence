using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Dialogs.Commands
{
    public class EditInfoDialogViewModel :
        ActionDialogViewModelBase<EditInfoDialogViewModel>
    {
        private IEditObjectWithInfo _Item;
        public IEditObjectWithInfo Item { get { return _Item; } set { Set(ref _Item, value); } }

        private string _Info;
        public string Info { get { return _Info; } set { Set(ref _Info, value); } }

        protected override EditInfoDialogViewModel This { get { return this; } }

        public override sealed string Title { 
            get {
                var key = string.Format(App.ResourceDictionary.StrObjectFrmt, _Item.GetType().Name);
                var frmt = App.ResourceDictionary.GetResource<string>(key);
                return string.Format(frmt, _Item.ID); 
            } 
        }

        public EditInfoDialogViewModel()
        {
        }
    }
}

