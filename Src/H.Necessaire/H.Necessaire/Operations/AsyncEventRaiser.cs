using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public class AsyncEventRaiser<TEventArgs> where TEventArgs : EventArgs
    {
        readonly object owner;
        readonly object eventHandlersCollectionLocker = new object();
        readonly bool isRaiseDoneInParallel = false;
        readonly List<AsyncEventHandler<TEventArgs>> eventHandlers = new List<AsyncEventHandler<TEventArgs>>();
        public AsyncEventRaiser(object owner, bool isRaiseDoneInParallel = false)
        {
            this.owner = owner;
            this.isRaiseDoneInParallel = isRaiseDoneInParallel;
        }

        public event AsyncEventHandler<TEventArgs> OnEvent
        {
            add => AddHandler(value);
            remove => ZapHandler(value);
        }

        public void AddHandler(AsyncEventHandler<TEventArgs> handler)
        {
            if (handler is null)
                return;

            lock (eventHandlersCollectionLocker)
            {
                eventHandlers.Add(handler);
            }
        }

        public void ZapHandler(AsyncEventHandler<TEventArgs> handler)
        {
            if (handler is null)
                return;

            lock (eventHandlersCollectionLocker)
            {
                eventHandlers.Remove(handler);
            }
        }

        public async Task Raise(TEventArgs args)
        {
            if (eventHandlers.IsEmpty())
                return;

            AsyncEventHandler<TEventArgs>[] currentEventHandlers = eventHandlers.ToArray();

            if (isRaiseDoneInParallel)
            {
                await RaiseEventsInParallel(args, currentEventHandlers);
            }
            else
            {
                await RaiseEventsInSyncedOrder(args, currentEventHandlers);
            }
        }

        private async Task RaiseEventsInSyncedOrder(TEventArgs args, AsyncEventHandler<TEventArgs>[] currentEventHandlers)
        {
            foreach (AsyncEventHandler<TEventArgs> handler in currentEventHandlers)
            {
                await HSafe.Run(async () => await handler.Invoke(owner, args));
            }
        }

        private async Task RaiseEventsInParallel(TEventArgs args, AsyncEventHandler<TEventArgs>[] currentEventHandlers)
        {
            await Task.WhenAll(
                currentEventHandlers.Select(
                    async handler => await HSafe.Run(async () => await handler.Invoke(owner, args))
                )
            );
        }
    }
}
