using System;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public static class TaskExtensions
    {
        public static Task<T> AsTask<T>(this T value) => Task.FromResult(value);
        public static Func<Task> AsAsync(this Action action)
        {
            return new Func<Task>(() => { action?.Invoke(); return true.AsTask(); });
        }

        public static async Task ThrowOnFail(this Task<OperationResult> operationResultTask)
            => (await operationResultTask).ThrowOnFail();
        public static async Task<T> ThrowOnFailOrReturn<T>(this Task<OperationResult<T>> operationResultTask)
            => (await operationResultTask).ThrowOnFailOrReturn();
        public static async Task<T> Return<T>(this Task<OperationResult<T>> operationResultTask)
            => (await operationResultTask).Payload;
    }
}
