namespace H.Necessaire.Runtime
{
    public static class ConsumerScopeExtensions
    {
        public static ConsumerScope Scope(this SyncRequest syncRequest) => new ConsumerScope(syncRequest);
        public static ConsumerScope Scope(this OperationContext operationContext) => new ConsumerScope(operationContext);
        public static ConsumerScope Scope(this ConsumerIdentity consumer) => new ConsumerScope(consumer);
    }
}
