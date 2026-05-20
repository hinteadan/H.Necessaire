using System;
using System.Threading;
using System.Threading.Tasks;

namespace H.Necessaire.Operations.Concrete
{
    internal class LimitedConcurrencyRunner : ImALimitedConcurrencyRunner, ImADependency
    {
        readonly int maxConcurrency;
        readonly SemaphoreSlim concurrencySemaphore;
        public LimitedConcurrencyRunner(int maxConcurrency = 150)
        {
            this.maxConcurrency = maxConcurrency <= 0 ? 1 : maxConcurrency;
            concurrencySemaphore = new SemaphoreSlim(this.maxConcurrency, this.maxConcurrency);
        }
        ImACancellationManager globalCancellationManager;
        public virtual void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            globalCancellationManager = dependencyProvider.Get<ImACancellationManager>();
        }

        public async Task Run(Func<Task> actionToRun)
        {
            if (actionToRun is null)
                return;

            using (await WaitForRunningSlot())
            {
                if (globalCancellationManager.Token.IsCancellationRequested)
                    return;

                await HSafe.Run(actionToRun);
            }
        }

        public async Task<OperationResult<IDisposable>> WaitIfNecessary()
        {
            if (globalCancellationManager.Token.IsCancellationRequested)
                return "Cancelling due to global cancellation request";

            var scope = await WaitForRunningSlot();

            if (globalCancellationManager.Token.IsCancellationRequested)
                return "Cancelling due to global cancellation request";

            return scope.ToWinResult();
        }

        async Task<IDisposable> WaitForRunningSlot()
        {
            await HSafe.Run(async () => await concurrencySemaphore.WaitAsync(globalCancellationManager.Token));

            if (globalCancellationManager.Token.IsCancellationRequested)
                return ScopedRunner.Null;

            return new ScopedRunner(null, _ => concurrencySemaphore.Release());
        }
    }
}
