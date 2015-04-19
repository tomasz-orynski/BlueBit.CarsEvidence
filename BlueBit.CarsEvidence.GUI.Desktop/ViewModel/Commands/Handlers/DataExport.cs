using BlueBit.CarsEvidence.BL.Repositories;
using System.Windows;
using Microsoft.Win32;
using BlueBit.CarsEvidence.BL.DTO.XML;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Commands.Handlers
{
    public class DataExportCommandHandler :
        DataCommandHandlerBase
    {
        private readonly IDbRepositories _entitiesRepository;

        public override CmdKey Key { get { return CmdKey.DataExport; } }

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
                Serialize(new DataEXP(_entitiesRepository), dlg.FileName);
                MessageBox.Show("Export finished.", "TODO", MessageBoxButton.OK);
            }
        }
    }
}
