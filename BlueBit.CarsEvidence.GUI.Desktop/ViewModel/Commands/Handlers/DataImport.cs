using dotNetExt;
using BlueBit.CarsEvidence.BL.DTO.XML;
using BlueBit.CarsEvidence.GUI.Desktop.Model;
using Microsoft.Win32;
using System.Linq;
using System.Windows;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Commands.Handlers
{
    public class DataImportCommandHandler :
        DataCommandHandlerBase
    {
        private readonly Repositories _repositories;

        public override CmdKey Key { get { return CmdKey.DataImport; } }

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
                DeSerialize<DataIMP>(dlg.FileName)
                    .GetEntities()
                    .Each(_repositories.Save);
                MessageBox.Show("Import finished.", "TODO", MessageBoxButton.OK);
            }
        }
    }
}
