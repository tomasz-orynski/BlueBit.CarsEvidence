
namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Panels
{
    public enum PanelIdentifier
    {
        Panel1,
        Panel2,
    }

    public interface IPanelViewModel :
        IDockViewModel
    {
        PanelIdentifier Identifier { get; }
    }

    public abstract class PanelViewModelBase : 
        ViewModelWithVisibilityBase,
        IPanelViewModel
    {
        public abstract PanelIdentifier Identifier { get; }
    }
}
