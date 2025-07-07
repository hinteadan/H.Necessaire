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
    }
}
