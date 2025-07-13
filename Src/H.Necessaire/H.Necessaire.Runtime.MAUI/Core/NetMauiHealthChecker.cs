namespace H.Necessaire.Runtime.MAUI.Core
{
    internal class NetMauiHealthChecker : HealthChecker
    {
        protected override Task<bool> HasSurelyNoInternet()
            => Connectivity.Current.NetworkAccess.In(NetworkAccess.None, NetworkAccess.Local, NetworkAccess.ConstrainedInternet).AsTask();

        protected override async Task<OperationResult> RunHttpRequestHealthCheck(string url)
        {
            return (await base.RunHttpRequestHealthCheck(url)).And(x => {
                x.WithComment(Connectivity.Current.ConnectionProfiles.Select(x => $"ConnectionProfile::{x}").Distinct().ToArray());
            });
        }
    }
}
