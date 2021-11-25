using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Integration.NetCore.Daemons
{
    internal class QdActionProcessingHostedServiceDaemon : IHostedService, ImADaemon, ImADependency
    {
        #region Construct
        ImADaemon qdActionProcessingDaemon;
        public virtual void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            qdActionProcessingDaemon = dependencyProvider.Get<QdActionProcessingDaemon>();
        }
        #endregion

        public Task Start(CancellationToken? cancellationToken = null) => qdActionProcessingDaemon.Start(cancellationToken);

        public Task StartAsync(CancellationToken cancellationToken) => Start(cancellationToken);

        public Task Stop(CancellationToken? cancellationToken = null) => qdActionProcessingDaemon.Stop(cancellationToken);

        public Task StopAsync(CancellationToken cancellationToken) => Stop(cancellationToken);
    }
}
