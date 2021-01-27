using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface INotify
    {
        Task<OperationResult> Send(NotificationMessage message, NotificationAddress from, params NotificationEndpoint[] to);
    }
}
