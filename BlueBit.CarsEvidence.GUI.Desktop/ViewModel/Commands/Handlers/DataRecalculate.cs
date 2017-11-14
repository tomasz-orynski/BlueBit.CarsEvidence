using BlueBit.CarsEvidence.BL.Entities;
using BlueBit.CarsEvidence.BL.Repositories;
using BlueBit.CarsEvidence.GUI.Desktop.Model;
using BlueBit.CarsEvidence.BL.Alghoritms;
using Microsoft.Win32;
using System.Linq;
using System.Windows;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Commands.Handlers
{
    public class DataRecalculateCommandHandler :
        DataCommandHandlerBase
    {
        private readonly IDbRepositories _repositories;

        public override CmdKey Key { get { return CmdKey.Recalculate; } }

        public DataRecalculateCommandHandler(IDbRepositories repositories)
        {
            _repositories = repositories;
        }

        public override void Execute()
        {
            var periods = _repositories
                .GetAll<Period>()
                .RecalculateStats();
            _repositories.Update(periods);
            MessageBox.Show("Recalculate finished.", "TODO", MessageBoxButton.OK);
        }
    }
}
