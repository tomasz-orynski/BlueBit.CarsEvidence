using BlueBit.CarsEvidence.BL.Entities;
using BlueBit.CarsEvidence.GUI.Desktop.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel
{
    public class ViewModelLocator
    {
        public MainWindowViewModel MainWindowViewModel { get { return Configuration.Settings.ResolveType<MainWindowViewModel>(); } }
    }
}