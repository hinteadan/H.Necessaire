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
        readonly Func<WebSocketSessionManager> webSocketSessionManagerRetriever;
        WebSocketSessionManager webSocketSessionManager => webSocketSessionManagerRetriever?.Invoke();
        public WebSocketServerToClientOperations(Func<WebSocketSessionManager> webSocketSessionManagerRetriever, Func<string, string> sessionIdViaUserIdRetriever)
        {
            this.sessionIdViaUserIdRetriever = sessionIdViaUserIdRetriever;
            this.webSocketSessionManagerRetriever = webSocketSessionManagerRetriever;
        }
        #endregion

        public Task<OperationResult> Broadcast(NotificationMessage message, NotificationAddress from)
        {
            TaskCompletionSource<OperationResult> taskCompletionSource = new TaskCompletionSource<OperationResult>();

            new Action(() =>
            {
                webSocketSessionManager.Broadcast(message.Content);

                taskCompletionSource.SetResult(OperationResult.Win());
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
                    OperationResult[] results =
                        await
                            Task.WhenAll(
                                to.Select(
                                    client => SendMessageToSingleClient(message, from, client)
                                )
                                .ToArray()
                            );

                    bool isSuccess = results.All(x => x.IsSuccessful);
                    string[] comments = results.SelectMany(x => x.FlattenReasons()).ToArray();
                    string reason = (!comments?.Any() ?? true) ? null : "There were some errors while sending message to requested destinations. See comments for details.";

                    result = isSuccess ? OperationResult.Win(reason, comments) : OperationResult.Fail(reason, comments);
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
                string sessionId = sessionIdViaUserIdRetriever(to.Address.Address) ?? sessionIdViaUserIdRetriever(to.Address.Name);

                if (sessionId == null)
                {
                    taskCompletionSource.SetResult(OperationResult.Fail($"There's no client connection for the given destination {(to.Address.Address ?? to.Address.Name)}"));
                    return;
                }

                webSocketSessionManager.SendTo(message.Content, sessionIdViaUserIdRetriever(to.Address.Address));

                taskCompletionSource.SetResult(OperationResult.Win());
            })
            .TryOrFailWithGrace(
                onFail: ex => taskCompletionSource.SetResult(OperationResult.Fail(ex))
            );

            return taskCompletionSource.Task;
        }
    }
}
