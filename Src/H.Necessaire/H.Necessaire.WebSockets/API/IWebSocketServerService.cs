using System.Threading;
using System.Threading.Tasks;

namespace H.Necessaire.WebSockets
{
    public interface IWebSocketServerService
    {
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
    }
}
