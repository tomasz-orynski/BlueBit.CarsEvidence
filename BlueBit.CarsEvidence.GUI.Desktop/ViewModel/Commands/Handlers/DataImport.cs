using BlueBit.CarsEvidence.BL;
using BlueBit.CarsEvidence.BL.Entities;
using BlueBit.CarsEvidence.GUI.Desktop.Model;
using Microsoft.Win32;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Commands.Handlers
{
    public class DataImportCommandHandler :
        DataCommandHandlerBase
    {
        [DataContract(Namespace = Consts.NamespaceEntities, IsReference = true)]
        public class Data : DataBase
        {
            [DataMember(Order = 0)]
            public Company Company { get; set; }
            [DataMember(Order = 1)]
            public Person[] Persons { get; set; }
            [DataMember(Order = 2)]
            public Car[] Cars { get; set; }
            [DataMember(Order = 3)]
            public Address[] Addresses { get; set; }
            [DataMember(Order = 4)]
            public Route[] Routes { get; set; }
            [DataMember(Order = 5)]
            public Period[] Periods { get; set; }

            public IEnumerable<EntityBase> GetEntities()
            {
                yield return Company;
                foreach (var x in Persons)
                {
                    x.Init();
                    yield return x;
                }
                foreach (var x in Cars)
                {
                    x.Init();
                    yield return x;
                }
                foreach (var x in Addresses)
                {
                    x.Init();
                    yield return x;
                }
                foreach (var x in Routes)
                {
                    x.Init();
                    x.AddressFrom.Routes.Add(x);
                    x.AddressTo.Routes.Add(x);
                    yield return x;
                }
                foreach (var x in Periods)
                {
                    x.Init();
                    x.Car.Periods.Add(x);
                    foreach(var e in x.PeriodEntries)
                    {
                        e.Period = x;
                        e.Person.PeriodEntries.Add(e);
                        e.Route.PeriodEntries.Add(e);
                    }
                    yield return x;
                }
            }
        }

        private readonly Repositories _repositories;

        public override CmdKey Key { get { return CmdKey.Import; } }

        public DataImportCommandHandler(Repositories repositories)
        {
            _repositories = repositories;
        }

        public override void Execute()
        {
            var xml = ".xml";
            var dlg = new OpenFileDialog()
            {
                FileName = App.ResourceDictionary.GetResource<string>(App.ResourceDictionary.StrApp),
                DefaultExt = xml,
                Filter = string.Format(App.ResourceDictionary.GetResource<string>(App.ResourceDictionary.StrFilterXML), xml),
            };

            if (dlg.ShowDialog() == true)
            {
                var root = DeSerialize<Data>(dlg.FileName);
                _repositories.Import(root.GetEntities().ToList());
                MessageBox.Show("Import finished.", "TODO", MessageBoxButton.OK);
            }
        }
    }
}
