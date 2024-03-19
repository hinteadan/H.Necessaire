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

            JsPromise.All(promise.AsArray())
            .then<T, T>(results => {
                taskCompletionSource.TrySetResult(results[0]);
                return results[0];
            }, err => {
                taskCompletionSource.TrySetCanceled();
                return default(T);
            })
            .@catch(ex => {
                taskCompletionSource.TrySetException(new OperationResultException(OperationResult.Fail(ex.ToString())));
            })
            ;

            return taskCompletionSource.Task;
        }
    }

    [External]
    [Name("Promise")]
    public static class JsPromise
    {
        [Name("all")]
        public static extern Promise<T[]> All<T>(Promise<T>[] promises);
    }
}
