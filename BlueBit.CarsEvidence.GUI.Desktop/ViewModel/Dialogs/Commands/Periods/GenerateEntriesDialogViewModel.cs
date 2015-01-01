using BlueBit.CarsEvidence.Commons.Reflection;
using BlueBit.CarsEvidence.Commons.Templates;
using EditModels = BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit.Documents;
using ViewModels = BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.View.General;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlueBit.CarsEvidence.GUI.Desktop.Model;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Dialogs.Commands.Periods
{
    public class GenerateEntriesDialogViewModel :
        ActionDialogViewModelBase<GenerateEntriesDialogViewModel>
    {
        public ObservableCollection<ViewModels.Route> AllRoutes { get { return _routes().Items; } }
        public ObservableCollection<ViewModels.Person> AllPersons { get { return _persons().Items; } }

        private EditModels.Period _Period;
        public EditModels.Period Period { get { return _Period; } set { Set(ref _Period, value); } }

        private ViewModels.Person _Person;
        public ViewModels.Person Person { get { return _Person; } set { Set(ref _Person, value); } }

        private ViewModels.Route _Route;
        public ViewModels.Route Route { get { return _Route; } set { Set(ref _Route, value); } }

        private bool _OnlyWorkDays;
        public bool OnlyWorkDays { get { return _OnlyWorkDays; } set { Set(ref _OnlyWorkDays, value); } }

        protected override GenerateEntriesDialogViewModel This { get { return this; } }

        public override sealed string Title { 
            get {
                var key = string.Format(App.ResourceDictionary.StrObjectFrmt, typeof(EditModels.Period).Name);
                var frmt = App.ResourceDictionary.GetResource<string>(key);
                return string.Format(frmt, _Period.ID); 
            } 
        }

        private readonly Func<IViewObjects<ViewModels.Person>> _persons;
        private readonly Func<IViewObjects<ViewModels.Route>> _routes;

        public GenerateEntriesDialogViewModel(
            Func<IViewObjects<ViewModels.Person>> persons,
            Func<IViewObjects<ViewModels.Route>> routes
            )
        {
            _persons = persons;
            _routes = routes;
        }
    }
}
