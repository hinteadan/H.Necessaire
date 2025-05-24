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
        readonly List<AsyncEventHandler<TEventArgs>> eventHandlers = new List<AsyncEventHandler<TEventArgs>>();
        public AsyncEventRaiser(object owner)
        {
            this.owner = owner;
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

            await Task.WhenAll(
                currentEventHandlers.Select(
                    async handler => await new Func<Task>(async () =>
                    {

                        await handler.Invoke(
                            owner,
                            args
                        );

                    })
                    .TryOrFailWithGrace(onFail: ex => { })
                )
            );


        }
    }
}
