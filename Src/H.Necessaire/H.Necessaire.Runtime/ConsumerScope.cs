using System;

namespace H.Necessaire.Runtime
{
    public class ConsumerScope : ScopedRunner
    {
        public ConsumerScope(ConsumerIdentity consumer)
            : base(
                onStart: () =>
                {
                    CallContext<ConsumerIdentity>.SetData(WellKnownCallContextKey.ConsumerIdentity, consumer);
                    CallContext<Guid?>.SetData(WellKnownCallContextKey.ConsumerID, consumer?.ID);
                },
                onStop: () =>
                {
                    CallContext<ConsumerIdentity>.ZapData(WellKnownCallContextKey.ConsumerIdentity);
                    CallContext<Guid?>.ZapData(WellKnownCallContextKey.ConsumerID);
                }
            )
        { }

        public ConsumerScope(OperationContext operationContext) : this(operationContext?.Consumer) { }
        public ConsumerScope(SyncRequest syncRequest) : this(syncRequest?.OperationContext) { }
    }
}
