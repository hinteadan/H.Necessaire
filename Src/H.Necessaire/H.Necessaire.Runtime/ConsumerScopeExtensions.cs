namespace H.Necessaire.Runtime
{
    public static class ConsumerScopeExtensions
    {
        public static ConsumerScope ConsumerScope(this SyncRequest syncRequest) => new ConsumerScope(syncRequest);
        public static ConsumerScope ConsumerScope(this OperationContext operationContext) => new ConsumerScope(operationContext);
        public static ConsumerScope Scope(this ConsumerIdentity consumer) => new ConsumerScope(consumer);
    }
}
