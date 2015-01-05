
namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Documents
{
    public interface IDocumentViewModel :
        IDockViewModel
    {
    }

    public abstract class DocumentViewModelBase :
        ViewModelWithCloseBase,
        IDocumentViewModel
    {
    }
}
