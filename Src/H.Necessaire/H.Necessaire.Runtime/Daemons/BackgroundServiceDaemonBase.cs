using System;
using System.Threading;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime
{
    public abstract class BackgroundServiceDaemonBase : ImADaemon, ImADependency
    {
        BackgroundServiceDaemon backgroundServiceDaemon;
        public virtual void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            backgroundServiceDaemon = new BackgroundServiceDaemon(dependencyProvider, DoWork, DoWorkInterval, StartDelay);
        }
        protected abstract TimeSpan DoWorkInterval { get; }
        protected abstract Task DoWork();

        protected virtual TimeSpan? StartDelay => null;

        public virtual async Task Start(CancellationToken? cancellationToken = null)
            => await backgroundServiceDaemon.Start(cancellationToken);

        public virtual async Task Stop(CancellationToken? cancellationToken = null)
            => await backgroundServiceDaemon.Stop(cancellationToken);
    }
}
