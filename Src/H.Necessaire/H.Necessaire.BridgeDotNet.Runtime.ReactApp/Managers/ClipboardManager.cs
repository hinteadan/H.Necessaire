using H.Necessaire.BridgeDotNet.Runtime.ReactApp.Core;
using System;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class ClipboardManager
    {
        public Task CopyToClipboard(string text)
        {
            TaskCompletionSource<string> taskCompletionSource = new TaskCompletionSource<string>();

            Clipboard.writeText(text).then(
                onfulfilled: x =>
                {
                    taskCompletionSource.TrySetResult(text);
                    return x;
                },
                onrejected: x =>
                {
                    taskCompletionSource.TrySetException(new InvalidOperationException("Cannot write to clipboard"));
                    return x;
                }
            );

            return taskCompletionSource.Task;
        }
    }
}
