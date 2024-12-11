using System;

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
    }
}
