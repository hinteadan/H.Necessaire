using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Integration.NetCore
{
    public class SyncRequestProcessingHostedServiceDaemon : IHostedService, ImADaemon, ImADependency
    {
        ImADaemon syncRequestProcessingDaemon = null;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            this.syncRequestProcessingDaemon = dependencyProvider.Get<SyncRequestProcessingDaemon>();
        }

        public Task StartAsync(CancellationToken cancellationToken) => Start(cancellationToken);

        public Task StopAsync(CancellationToken cancellationToken) => Stop(cancellationToken);

        public Task Start(CancellationToken? cancellationToken = null) => syncRequestProcessingDaemon?.Start(cancellationToken);

        public Task Stop(CancellationToken? cancellationToken = null) => syncRequestProcessingDaemon?.Stop(cancellationToken);
    }
}
