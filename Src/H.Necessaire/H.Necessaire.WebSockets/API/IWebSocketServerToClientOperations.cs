using System.Threading.Tasks;

namespace H.Necessaire.WebSockets
{
    public interface IWebSocketServerToClientOperations : INotify
    {
        Task<OperationResult> Broadcast(NotificationMessage message, NotificationAddress from);
    }
}
