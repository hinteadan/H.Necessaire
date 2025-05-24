using System.Threading.Tasks;

namespace H.Necessaire.Runtime.UI
{
    public interface ImAnHUIRefresher
    {
        Task RefreshUI(object nativeViewReference, HViewModel viewModel, HViewModelChangedEventArgs hViewModelChangedEventArgs);
    }
}
