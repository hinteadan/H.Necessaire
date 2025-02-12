using System.Threading.Tasks;

namespace H.Necessaire.Runtime.UI.Abstractions
{
    public interface ImAnHUIComponent
    {
        void Construct();
        void ReferenceNativeView(object nativeViewReference);
        Task Initialize();
        Task InitializeAndBindViewModel();
        Task Destroy();
        Task RefreshUI();

        HViewModel ViewModel { get; }
    }
}
