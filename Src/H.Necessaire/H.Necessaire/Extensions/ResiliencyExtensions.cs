using System;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public static class ResiliencyExtensions
    {
        public static OperationResult<TResult> TryRead<TData, TResult>(this TData payload, Func<TData, TResult> dataSelector, TResult fallbakValue = default)
        {
            if (dataSelector is null)
                return OperationResult.Fail($"{nameof(dataSelector)} not specified").WithPayload(fallbakValue);

            OperationResult<TResult> result = OperationResult.Fail("Not yet started").WithPayload(fallbakValue);

            new Action(() =>
            {

                TResult resultValue = dataSelector.Invoke(payload);

                result = OperationResult.Win().WithPayload(resultValue);

            })
            .TryOrFailWithGrace(onFail:
                ex => result
                    = OperationResult
                    .Fail(ex, $"Error occurred while trying to ReadOrFallbackTo. Reason: {ex.Message}")
                    .WithPayload(fallbakValue)
            );

            return result;
        }

        public static TResult SafeRead<TData, TResult>(this TData payload, Func<TData, TResult> dataSelector, TResult fallbakValue = default)
        {
            return
                payload.TryRead(dataSelector, fallbakValue).Payload;
        }

        public static OperationResult Then(this OperationResult currentResult, Func<OperationResult> doThis)
        {
            if (currentResult is null)
                return OperationResult.Fail("currentResult is null");

            if (!currentResult.IsSuccessful)
                return currentResult;

            if (doThis is null)
                return currentResult;

            return doThis.Invoke();
        }

        public static async Task<OperationResult> Then(this Task<OperationResult> currentResult, Func<Task<OperationResult>> doThis)
        {
            if (currentResult is null)
                return OperationResult.Fail("currentResult is null");

            OperationResult current = await currentResult;

            if (current is null)
                return OperationResult.Fail("currentResult is null");

            if (!current.IsSuccessful)
                return current;

            if (doThis is null)
                return current;

            return await doThis.Invoke();
        }

        public static async Task<OperationResult<TNext>> Then<T, TNext>(this Task<OperationResult<T>> currentResult, Func<T, Task<OperationResult<TNext>>> doThis)
        {
            if (currentResult is null)
                return OperationResult.Fail("currentResult is null").WithoutPayload<TNext>();

            OperationResult<T> current = await currentResult;

            if (current is null)
                return OperationResult.Fail("currentResult is null").WithoutPayload<TNext>();

            if (!current.IsSuccessful)
                return current.WithoutPayload<TNext>();

            if (doThis is null)
                return current.WithoutPayload<TNext>();

            return await doThis.Invoke(current.Payload);
        }
    }
}
