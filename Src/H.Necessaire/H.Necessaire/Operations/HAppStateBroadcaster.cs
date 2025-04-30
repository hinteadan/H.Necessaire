using System;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public class HAppStateBroadcaster
    {
        public static readonly HAppStateBroadcaster Default = new HAppStateBroadcaster();

        readonly AsyncEventRaiser<EventArgs> eventRaiser = null;
        public HAppStateBroadcaster()
        {
            eventRaiser = new AsyncEventRaiser<EventArgs>(this);
        }

        public event AsyncEventHandler<EventArgs> OnAppStateChanged { add => eventRaiser.OnEvent += value; remove => eventRaiser.OnEvent -= value; }

        public async Task BroadcastAppStateChange()
        {
            await eventRaiser.Raise(EventArgs.Empty);
        }
    }

    public class HAppStateBroadcaster<TEventArgs> where TEventArgs : EventArgs
    {
        public static readonly HAppStateBroadcaster<TEventArgs> Default = new HAppStateBroadcaster<TEventArgs>();

        readonly AsyncEventRaiser<TEventArgs> eventRaiser = null;
        public HAppStateBroadcaster()
        {
            eventRaiser = new AsyncEventRaiser<TEventArgs>(this);
        }

        public event AsyncEventHandler<TEventArgs> OnAppStateChanged { add => eventRaiser.OnEvent += value; remove => eventRaiser.OnEvent -= value; }

        public async Task BroadcastAppStateChange(TEventArgs eventArgs)
        {
            await eventRaiser.Raise(eventArgs);
        }
    }
}
