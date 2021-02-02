using H.Necessaire.WebSockets.Concrete.Model;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp.Server;

namespace H.Necessaire.WebSockets.Concrete
{
    class WebSocketServerService : IWebSocketServerService
    {
        #region Construct
        readonly WebSocketServer webSocketServer;
        readonly WebSocketClientNotificationQueue notificationQueue;

        IWebSocketServerToClientOperations serverToClientOperations = null;

        public WebSocketServerService(string webSocketServerBaseUrl, WebSocketClientNotificationQueue notificationQueue)
        {
            this.notificationQueue = notificationQueue;

            webSocketServer = new WebSocketServer(webSocketServerBaseUrl);
            webSocketServer.AddWebSocketService<ClientNotificationsBehavior>("/", x => x.Initialize(opts =>
            {
                serverToClientOperations = opts;
            }));
        }
        #endregion

        public Task StartAsync(CancellationToken cancellationToken)
        {
            webSocketServer.Start();

            BackgroundProcessing(cancellationToken);

            return true.AsTask();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            webSocketServer.Stop();

            return true.AsTask();
        }

        private async void BackgroundProcessing(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                ClientNotificationQueueEntry notificationEntry = await notificationQueue.DequeueAsync(cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                    return;

                await
                    new Func<Task>(async () =>
                    {
                        OperationResult result =
                            await serverToClientOperations.Send(notificationEntry.Message, notificationEntry.From, notificationEntry.To ?? new NotificationEndpoint[0]);
                    })
                    .TryOrFailWithGrace(
                        numberOfTimes: 1,
                        onFail: ex => { }
                    );
            }
        }
    }
}
