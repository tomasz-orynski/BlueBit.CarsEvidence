using BlueBit.CarsEvidence.Commons.Diagnostics;
using BlueBit.CarsEvidence.GUI.Desktop.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Commands.Handlers
{
    public interface ISettingsCommandHandler :
        ICommandHandler
    {
    }

    [Register(typeof(ISettingsCommandHandler), true)]
    public class SettingsCommandHandler :
        ISettingsCommandHandler
    {
#if DEBUG
        private readonly object _dbgSc = new SingletonChecker<SettingsCommandHandler>();
#endif

        public SettingsCommandHandler()
        {
        }

        public bool CanExecute()
        {
            return true;
        }

        public void Execute()
        {
            MessageBox.Show("Ustawienia.", "TODO", MessageBoxButton.OK);
        }
    }
}
