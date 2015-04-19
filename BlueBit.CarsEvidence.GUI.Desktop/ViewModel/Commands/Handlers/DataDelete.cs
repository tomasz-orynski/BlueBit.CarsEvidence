using BlueBit.CarsEvidence.GUI.Desktop.Model;
using System.Windows;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Commands.Handlers
{
    public class DataDeleteCommandHandler :
        DataCommandHandlerBase
    {
        private readonly Repositories _repositories;

        public override CmdKey Key { get { return CmdKey.DataDelete; } }

        public DataDeleteCommandHandler(Repositories repositories)
        {
            _repositories = repositories;
        }

        public override void Execute()
        {
            if (MessageBox.Show("Delete all data in DB?", "TODO", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _repositories.DeleteAll();
                MessageBox.Show("Clear DB finished.", "TODO", MessageBoxButton.OK);
            }
        }
    }
}
