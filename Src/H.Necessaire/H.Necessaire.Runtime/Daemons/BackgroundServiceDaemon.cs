using System;
using System.Threading;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime
{
    public class BackgroundServiceDaemon : ImADaemon
    {
        readonly Func<Task> backgroundTask;
        readonly TimeSpan? startDelay = null;
        readonly TimeSpan backgroundTaskInterval = Timeout.InfiniteTimeSpan;
        readonly ImALogger log;
        readonly ImAPeriodicAction periodicAction;
        readonly ImACancellationManager cancellationManager;
        CancellationTokenSource killSwitch = null;
        readonly SemaphoreSlim startStopSemaphore = new SemaphoreSlim(1, 1);
        bool isStarted = false;
        public BackgroundServiceDaemon(ImADependencyProvider dependencyProvider, Func<Task> backgroundTask, TimeSpan backgroundTaskInterval, TimeSpan? startDelay = null)
        {
            this.backgroundTask = backgroundTask;
            this.startDelay = startDelay;
            this.backgroundTaskInterval = backgroundTaskInterval;
            this.log = dependencyProvider.GetLogger<BackgroundServiceDaemon>();
            this.periodicAction = dependencyProvider.Get<ImAPeriodicAction>();
            this.cancellationManager = dependencyProvider.Get<ImACancellationManager>();
        }

        public virtual async Task Start(CancellationToken? cancellationToken = null)
        {
            if (backgroundTask is null)
                return;

            if (isStarted)
                return;

            ReArm(cancellationToken);

            await startStopSemaphore.WaitAsync();
            using (new ScopedRunner(null, () => { isStarted = true; startStopSemaphore.Release(); }))
            {
                if (isStarted)
                    return;

                if (startDelay != null)
                    periodicAction.StartDelayed(startDelay.Value, backgroundTaskInterval, SafelyRunbBackgroundTask);
                else
                    periodicAction.Start(backgroundTaskInterval, SafelyRunbBackgroundTask);
            }
        }

        public virtual async Task Stop(CancellationToken? cancellationToken = null)
        {
            if (backgroundTask is null)
                return;

            if (!isStarted)
                return;

            await startStopSemaphore.WaitAsync();
            using (new ScopedRunner(null, () => { isStarted = false; startStopSemaphore.Release(); }))
            {
                if (!isStarted)
                    return;

                periodicAction.Stop();
            }
        }

        async Task SafelyRunbBackgroundTask()
        {
            await HSafe.Run(async () => await backgroundTask()).LogError(log, nameof(SafelyRunbBackgroundTask));
        }

        void ReArm(CancellationToken? cancellationToken = null)
        {
            Interlocked.Exchange(
                ref killSwitch,
                CancellationTokenSource.CreateLinkedTokenSource(cancellationManager?.Token ?? CancellationToken.None, cancellationToken ?? CancellationToken.None)
                .And(x => x.Token.Register(async () => { await Stop(); }))
            );
        }
    }
}
