using BlueBit.CarsEvidence.BL.Entities;
using BlueBit.CarsEvidence.BL.Repositories;
using System.Windows;
using System.Linq;
using System.Runtime.Serialization;
using BlueBit.CarsEvidence.BL;
using Microsoft.Win32;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Commands.Handlers
{
    public class DataExportCommandHandler :
        DataCommandHandlerBase
    {
        [DataContract(Namespace = Consts.NamespaceEntities, IsReference = true)]
        public class Data : 
            DataBase
        {
            private readonly IDbRepositories _entitiesRepository;

            public Data(
                IDbRepositories entitiesRepository
            )
            {
                _entitiesRepository = entitiesRepository;
            }

            [DataMember(Order = 0)]
            public Company Company { get { return _entitiesRepository.GetAll<Company>().First(); } set { } }
            [DataMember(Order = 1)]
            public Person[] Persons { get { return _entitiesRepository.GetAll<Person>().ToArray(); } }
            [DataMember(Order = 2)]
            public Car[] Cars { get { return _entitiesRepository.GetAll<Car>().ToArray(); } }
            [DataMember(Order = 3)]
            public Address[] Addresses { get { return _entitiesRepository.GetAll<Address>().ToArray(); } }
            [DataMember(Order = 4)]
            public Route[] Routes { get { return _entitiesRepository.GetAll<Route>().ToArray(); } }
            [DataMember(Order = 5)]
            public Period[] Periods { get { return _entitiesRepository.GetAll<Period>().ToArray(); } }
        }


        private readonly IDbRepositories _entitiesRepository;

        public override CmdKey Key { get { return CmdKey.Export; } }

        public DataExportCommandHandler(
            IDbRepositories entitiesRepository
            )
        {
            _entitiesRepository = entitiesRepository;
        }

        public override void Execute()
        {
            var xml = ".xml";
            var dlg = new SaveFileDialog() {
                FileName = App.ResourceDictionary.GetResource<string>(App.ResourceDictionary.StrApp),
                DefaultExt = xml,
                Filter = string.Format(App.ResourceDictionary.GetResource<string>(App.ResourceDictionary.StrFilterXML), xml),
            };

            if (dlg.ShowDialog() == true)
            {
                Serialize(new Data(_entitiesRepository), dlg.FileName);
                MessageBox.Show("Export finished.", "TODO", MessageBoxButton.OK);
            }
        }
    }
}
