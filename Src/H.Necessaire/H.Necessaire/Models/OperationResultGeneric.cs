namespace H.Necessaire
{
    public class OperationResult<T> : OperationResult
    {
        public T Payload { get; set; }

        public OperationResult() { }

        public OperationResult(OperationResult operationResult)
        {
            IsSuccessful = operationResult.IsSuccessful;
            Reason = operationResult.Reason;
            Comments = operationResult.Comments ?? new string[0];
        }

        public OperationResult(OperationResult operationResult, T payload) : this(operationResult)
        {
            Payload = payload;
        }

        public T ThrowOnFailOrReturn()
        {
            ThrowOnFail();

            return Payload;
        }
    }
}
