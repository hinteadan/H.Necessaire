using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Integration.NetCore
{
    public abstract class HostedServiceDaemonBase : IHostedService, ImADaemon, ImADependency
    {
        #region Construct
        static readonly TimeSpan defaultWorkCycleInterval = TimeSpan.FromSeconds(30);
        ImAPeriodicAction periodicAction;
        ImALogger logger;
        public virtual void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            periodicAction = dependencyProvider.Get<ImAPeriodicAction>();
            logger = dependencyProvider.GetLogger(this.GetType(), "H.Necessaire.Runtime.Integration.NetCore");
        }
        #endregion

        protected ImALogger Logger => logger;

        public Task StartAsync(CancellationToken cancellationToken) => Start(cancellationToken);

        public Task StopAsync(CancellationToken cancellationToken) => Stop(cancellationToken);


        public virtual Task Start(CancellationToken? cancellationToken = null)
        {
            periodicAction.Start(WorkCycleInterval, async () => await DoWork(cancellationToken));
            return true.AsTask();
        }

        public virtual Task Stop(CancellationToken? cancellationToken = null)
        {
            periodicAction.Stop();
            return true.AsTask();
        }

        protected abstract Task DoWork(CancellationToken? cancellationToken = null);

        protected virtual TimeSpan WorkCycleInterval { get; } = defaultWorkCycleInterval;
    }
}
