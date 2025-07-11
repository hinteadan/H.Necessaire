namespace H.Necessaire.Runtime.MAUI.Core
{
    internal class NetMauiHealthChecker : HealthChecker
    {
        protected override Task<bool> HasSurelyNoInternet()
            => Connectivity.Current.NetworkAccess.In(NetworkAccess.None, NetworkAccess.Local, NetworkAccess.ConstrainedInternet).AsTask();
    }
}
