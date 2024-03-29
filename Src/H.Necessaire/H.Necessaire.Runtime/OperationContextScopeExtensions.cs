﻿namespace H.Necessaire.Runtime
{
    public static class OperationContextScopeExtensions
    {
        public static OperationContextScope OperationContextScope(this SyncRequest syncRequest) => new OperationContextScope(syncRequest);
        public static OperationContextScope Scope(this OperationContext operationContext) => new OperationContextScope(operationContext);
    }
}
