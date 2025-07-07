namespace H.Necessaire
{
    public class OperationResult<T> : OperationResult
    {
        public T Payload { get; set; }

        public OperationResult() { }

        public OperationResult(OperationResult operationResult)
        {
            IsSuccessful = operationResult.IsSuccessful;
            Reason = operationResult.Reason.NullIfEmpty();
            Comments = operationResult.Comments.ToNonEmptyArray();
            ReasonsToDisplay = operationResult.ReasonsToDisplay.ToNonEmptyArray();
            Warnings = operationResult.Warnings.ToNonEmptyArray();
        }

        public OperationResult(OperationResult operationResult, T payload) : this(operationResult)
        {
            Payload = payload;
        }

        public new void ThrowOnFail()
        {
            if (IsSuccessful)
                return;

            throw new OperationResultException<T>(this);
        }

        public T ThrowOnFailOrReturn()
        {
            ThrowOnFail();

            return Payload;
        }

        public static implicit operator bool(OperationResult<T> operationResult) => operationResult?.IsSuccessful == true;
        public static implicit operator OperationResult<T>(T payload) => payload.ToWinResult();
        public static implicit operator OperationResult<T>(string failReason) => Fail(failReason).WithoutPayload<T>();

        public T Return() => Payload;
        public OperationResult<T> RefResult(out OperationResult<T> result)
        {
            result = this;
            return this;
        }
        public OperationResult<T> RefPayload(out T payload)
        {
            payload = Payload;
            return this;
        }

        public OperationResult<T> Ref(out OperationResult<T> result, out T payload)
        {
            result = this;
            payload = Payload;
            return this;
        }
    }
}
