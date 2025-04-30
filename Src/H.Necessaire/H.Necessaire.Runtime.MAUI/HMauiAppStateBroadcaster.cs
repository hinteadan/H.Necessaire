namespace H.Necessaire.Runtime.MAUI
{
    public class HMauiAppStateBroadcaster
    {
        public static readonly HMauiAppStateBroadcaster Default = new HMauiAppStateBroadcaster();

        readonly AsyncEventRaiser<EventArgs> viewModelChangeEventRaiser = null;
        public HMauiAppStateBroadcaster()
        {
            viewModelChangeEventRaiser = new AsyncEventRaiser<EventArgs>(this);
        }

        public event AsyncEventHandler<EventArgs> OnAppStateChanged { add => viewModelChangeEventRaiser.OnEvent += value; remove => viewModelChangeEventRaiser.OnEvent -= value; }

        public async Task BroadcastAppStateChange()
        {
            await viewModelChangeEventRaiser.Raise(EventArgs.Empty);
        }
    }

    public class HMauiAppStateBroadcaster<TEventArgs> where TEventArgs : EventArgs
    {
        public static readonly HMauiAppStateBroadcaster<TEventArgs> Default = new HMauiAppStateBroadcaster<TEventArgs>();

        readonly AsyncEventRaiser<TEventArgs> viewModelChangeEventRaiser = null;
        public HMauiAppStateBroadcaster()
        {
            viewModelChangeEventRaiser = new AsyncEventRaiser<TEventArgs>(this);
        }

        public event AsyncEventHandler<TEventArgs> OnAppStateChanged { add => viewModelChangeEventRaiser.OnEvent += value; remove => viewModelChangeEventRaiser.OnEvent -= value; }

        public async Task BroadcastAppStateChange(TEventArgs eventArgs)
        {
            await viewModelChangeEventRaiser.Raise(eventArgs);
        }
    }
}
