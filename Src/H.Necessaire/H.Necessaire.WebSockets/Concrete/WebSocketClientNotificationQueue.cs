using H.Necessaire.WebSockets.Concrete.Model;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace H.Necessaire.WebSockets.Concrete
{
    class WebSocketClientNotificationQueue : IWebSocketClientNotifier
    {
        #region Construct
        readonly ConcurrentQueue<ClientNotificationQueueEntry> notificationQueue = new ConcurrentQueue<ClientNotificationQueueEntry>();
        readonly SemaphoreSlim processingSemaphore = new SemaphoreSlim(0);
        #endregion

        public Task<OperationResult> Send(NotificationMessage message, NotificationAddress from, params NotificationEndpoint[] to)
        {
            ClientNotificationQueueEntry queueEntry
                = new ClientNotificationQueueEntry
                {
                    From = from,
                    Message = message,
                    To = to ?? new NotificationEndpoint[0],
                };

            notificationQueue.Enqueue(queueEntry);
            processingSemaphore.Release();
            return OperationResult.Win().AsTask();
        }

        public async Task<ClientNotificationQueueEntry> DequeueAsync(CancellationToken stoppingToken)
        {
            await processingSemaphore.WaitAsync(stoppingToken);

            if (stoppingToken.IsCancellationRequested)
                return ClientNotificationQueueEntry.None;

            notificationQueue.TryDequeue(out ClientNotificationQueueEntry entry);
            return entry ?? ClientNotificationQueueEntry.None;
        }
    }
}
