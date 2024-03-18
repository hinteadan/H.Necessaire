using Bridge;
using System;
using System.Threading.Tasks;
using static Retyped.es5;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    [Module(ModuleType.UMD, nameof(HServiceWorker))]
    public static class HServiceWorkerExtensions
    {
        public static Task<T> ToAsync<T>(this Promise<T> promise)
        {
            if (promise == null)
                return (default(T)).AsTask();

            TaskCompletionSource<T> taskCompletionSource = new TaskCompletionSource<T>();

            promise
                .then<T, T>(
                    result => {
                        taskCompletionSource.SetResult(result);
                        return result;
                    },
                    ex => {
                        taskCompletionSource.SetCanceled();
                        return default(T);
                    })
                .@catch(ex => {
                    taskCompletionSource.SetException(new InvalidOperationException(ex.ToString()));
                });

            return taskCompletionSource.Task;
        }
    }
}
