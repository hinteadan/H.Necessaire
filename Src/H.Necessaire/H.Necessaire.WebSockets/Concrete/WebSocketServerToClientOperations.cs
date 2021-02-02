using System;
using System.Linq;
using System.Threading.Tasks;
using WebSocketSharp.Server;

namespace H.Necessaire.WebSockets.Concrete
{
    class WebSocketServerToClientOperations : IWebSocketServerToClientOperations
    {
        #region Construct
        readonly Func<string, string> sessionIdViaUserIdRetriever;
        readonly WebSocketSessionManager webSocketSessionManager;
        public WebSocketServerToClientOperations(WebSocketSessionManager webSocketSessionManager, Func<string, string> sessionIdViaUserIdRetriever)
        {
            this.sessionIdViaUserIdRetriever = sessionIdViaUserIdRetriever;
            this.webSocketSessionManager = webSocketSessionManager;
        }
        #endregion

        public Task<OperationResult> Broadcast(NotificationMessage message, NotificationAddress from)
        {
            TaskCompletionSource<OperationResult> taskCompletionSource = new TaskCompletionSource<OperationResult>();

            new Action(() =>
            {
                webSocketSessionManager.BroadcastAsync(message.Encoding.GetBytes(message.Content), () =>
                {
                    taskCompletionSource.SetResult(OperationResult.Win());
                });
            })
            .TryOrFailWithGrace(
                onFail: ex => taskCompletionSource.SetResult(OperationResult.Fail(ex))
            );

            return taskCompletionSource.Task;
        }

        public async Task<OperationResult> Send(NotificationMessage message, NotificationAddress from, params NotificationEndpoint[] to)
        {
            if (!to?.Any() ?? true)
                return OperationResult.Win("No destinations to send the message to");

            OperationResult result = OperationResult.Win();

            await
                new Func<Task>(async () =>
                {
                    await
                        Task.WhenAll(
                            to.Select(
                                client => SendMessageToSingleClient(message, from, client)
                            )
                            .ToArray()
                        );

                })
                .TryOrFailWithGrace(
                    onFail: ex => result = OperationResult.Fail(ex)
                );

            return result;
        }

        private Task<OperationResult> SendMessageToSingleClient(NotificationMessage message, NotificationAddress from, NotificationEndpoint to)
        {
            TaskCompletionSource<OperationResult> taskCompletionSource = new TaskCompletionSource<OperationResult>();

            new Action(() =>
            {
                webSocketSessionManager.SendToAsync(message.Encoding.GetBytes(message.Content), sessionIdViaUserIdRetriever(to.Address.Address), isSuccessful =>
                {
                    taskCompletionSource.SetResult(isSuccessful ? OperationResult.Win() : OperationResult.Fail($"Cannot reach client {to.Address.Address}"));
                });
            })
            .TryOrFailWithGrace(
                onFail: ex => taskCompletionSource.SetResult(OperationResult.Fail(ex))
            );

            return taskCompletionSource.Task;
        }
    }
}
