using BlueBit.CarsEvidence.BL.Alghoritms;
using BlueBit.CarsEvidence.BL.Entities;
using BlueBit.CarsEvidence.BL.Repositories;
using BlueBit.CarsEvidence.GUI.Desktop.Model;
using System.Windows;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Commands.Handlers
{
    public class DataRecalculateCommandHandler :
        DataCommandHandlerBase
    {
        private readonly Repositories _repositories;
        private readonly DbRepositories _dbRepositories;

        public override CmdKey Key { get { return CmdKey.DataRecalculate; } }

        public DataRecalculateCommandHandler(
            Repositories repositories,
            DbRepositories dbRepositories)
        {
            _repositories = repositories;
            _dbRepositories = dbRepositories;
        }

        public override void Execute()
        {
            if (MessageBox.Show("Recalculate all data in DB?", "TODO", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                foreach (var car in _dbRepositories.GetAll<Car>())
                {
                    var results = car.Periods.RecalculateStats();
                    _dbRepositories.Update(results);
                }
                MessageBox.Show("Recalculate DB finished.", "TODO", MessageBoxButton.OK);
            }
        }
    }
}
