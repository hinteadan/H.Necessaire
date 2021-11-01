using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public interface ImAUiComponentState
    {
        Task Initialize();

        Task Use(UiNavigationParams uiNavigationParams);

        Task Destroy();
    }
}
