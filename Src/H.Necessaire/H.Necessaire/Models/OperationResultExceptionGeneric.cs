namespace H.Necessaire
{
    public class OperationResultException<T> : OperationResultException
    {
        public OperationResultException(OperationResult<T> operationResult)
            : base(operationResult)
        {
            OperationResult = operationResult;
        }

        public new OperationResult<T> OperationResult { get; }
    }
}
