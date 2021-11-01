
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public interface ImAUiComponent
    {
        bool IsBusy { get; }
        Task Initialize();
        Task Destroy();
        Task RunAtStartup();
    }
}
