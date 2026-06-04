using System.Linq;

namespace H.Necessaire
{
    public static class OperationResultExtensions
    {
        public static T Warn<T>(this T operationResult, params string[] warnings) where T : OperationResult
        {
            if (operationResult is null)
                return operationResult;

            if (warnings.IsEmpty())
                return operationResult;

            operationResult.Warnings = operationResult.Warnings.Push(warnings.ToNonEmptyArray()).NullIfEmpty();

            return operationResult;
        }

        public static T Display<T>(this T operationResult, params string[] reasonsToDisplay) where T : OperationResult
        {
            if (operationResult is null)
                return operationResult;

            if (reasonsToDisplay.IsEmpty())
                return operationResult;

            operationResult.ReasonsToDisplay = operationResult.ReasonsToDisplay.Push(reasonsToDisplay.ToNonEmptyArray()).NullIfEmpty();

            return operationResult;
        }

        public static OperationResult UnwrapToFirstFailOrLastWin(this OperationResult<OperationResult> opRes)
        {
            if (opRes == null)
                return opRes;

            if (!opRes.IsSuccessful)
                return opRes;

            return opRes.Payload;
        }

        public static OperationResult<T> UnwrapToFirstFailOrLastWin<T>(this OperationResult<OperationResult<T>> opRes)
        {
            if (opRes == null)
                return opRes?.WithoutPayload<T>();

            if (!opRes.IsSuccessful)
                return opRes.WithoutPayload<T>();

            return opRes.Payload;
        }

        public static OperationResult DeepUnwrapToFirstFailOrLastWin<T> (this OperationResult<T> opRes) where T : OperationResult
        {
            if (opRes == null)
                return opRes;

            if (!opRes.IsSuccessful)
                return opRes;

            var innerOpRes = opRes.Payload;

            if (innerOpRes == null)
                return innerOpRes;

            if (innerOpRes is OperationResult<T> deeperOpRes)
                return deeperOpRes.DeepUnwrapToFirstFailOrLastWin();

            return innerOpRes;
        }

        public static OperationResult<TResult> DeepUnwrapToFirstFailOrLastWin<T, TResult>(this OperationResult<T> opRes) where T : OperationResult<TResult>
        {
            if (opRes == null)
                return opRes.WithoutPayload<TResult>();

            if (!opRes.IsSuccessful)
                return opRes.WithoutPayload<TResult>();

            var innerOpRes = opRes.Payload;

            if (innerOpRes == null)
                return innerOpRes.WithoutPayload<TResult>();

            if (innerOpRes is OperationResult<T> deeperOpRes)
                return deeperOpRes.DeepUnwrapToFirstFailOrLastWin<T, TResult>();

            return innerOpRes;
        }
    }
}
