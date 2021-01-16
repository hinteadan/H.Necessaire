using System;
using System.Linq;

namespace H.Necessaire
{
    public class OperationResultException : AggregateException
    {
        public OperationResultException(OperationResult operationResult)
            : base(operationResult?.Reason, operationResult?.Comments?.Select(m => new InvalidOperationException(m)).ToArray() ?? new InvalidOperationException[0])
        {
            OperationResult = operationResult;
        }

        public OperationResult OperationResult { get; }
    }
}
