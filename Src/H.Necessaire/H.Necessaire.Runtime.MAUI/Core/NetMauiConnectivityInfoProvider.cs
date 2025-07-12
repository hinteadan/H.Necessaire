namespace H.Necessaire.Runtime.MAUI.Core
{
    internal class NetMauiConnectivityInfoProvider : HsConnectivityInfoProvider, ImADependency, IDisposable
    {
        readonly SemaphoreSlim connectivityChangedSemaphore = new SemaphoreSlim(1, 1);

        public NetMauiConnectivityInfoProvider()
        {
            Connectivity.Current.ConnectivityChanged += ConnectivityChanged;
        }
        ~NetMauiConnectivityInfoProvider() => HSafe.Run(Dispose);

        public void Dispose()
        {
            Connectivity.Current.ConnectivityChanged -= ConnectivityChanged;
        }

        async void ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            await connectivityChangedSemaphore.WaitAsync();
            try
            {
                await ForceRefresh();
            }
            finally
            {
                connectivityChangedSemaphore.Release();
            }
        }
    }
}
