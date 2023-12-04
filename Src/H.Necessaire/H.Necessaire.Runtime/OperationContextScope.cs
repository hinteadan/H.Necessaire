namespace H.Necessaire.Runtime
{
    public class OperationContextScope : ScopedRunner
    {
        public OperationContextScope(OperationContext operationContext)
            : base(
                onStart: () =>
                {
                    CallContext<OperationContext>.SetData(WellKnownCallContextKey.OperationContext, operationContext);
                },
                onStop: () =>
                {
                    CallContext<OperationContext>.ZapData(WellKnownCallContextKey.OperationContext);
                }
            )
        { }

        public OperationContextScope(SyncRequest syncRequest) : this(syncRequest?.OperationContext) { }
    }
}
