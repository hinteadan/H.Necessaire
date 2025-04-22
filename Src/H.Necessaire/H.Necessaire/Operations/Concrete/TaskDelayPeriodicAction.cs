using System;
using System.Threading;
using System.Threading.Tasks;

namespace H.Necessaire.Operations.Concrete
{
    class TaskDelayPeriodicAction : ImAPeriodicAction
    {
        readonly object isStartedLocker = new object();
        bool isStarted = false;
        TimeSpan interval = TimeSpan.Zero;
        Func<Task> action = null;
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        CancellationToken CancellationToken => cancellationTokenSource.Token;

        public void Start(TimeSpan interval, Func<Task> action)
            => StartDelayed(TimeSpan.Zero, interval, action);

        public async void StartDelayed(TimeSpan delay, TimeSpan interval, Func<Task> action)
        {
            if (action is null)
                return;

            lock (isStartedLocker)
            {
                if (isStarted)
                    return;

                isStarted = true;
            }

            this.action = action;
            this.interval = interval <= TimeSpan.Zero ? TimeSpan.Zero : interval;

            try
            {
                await Task.Delay(delay, CancellationToken);
            }
            catch(TaskCanceledException)
            {
                return;
            }

            await action();

            await DelayRunAndQueueAnother();
        }

        public void Stop()
        {
            lock (isStartedLocker)
            {
                if (!isStarted)
                    return;

                isStarted = false;
            }

            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = new CancellationTokenSource();
        }

        async Task DelayRunAndQueueAnother()
        {
            lock (isStartedLocker)
            {
                if (!isStarted)
                    return;
            }

            if (action is null)
                return;

            if (interval <= TimeSpan.Zero)
                return;

            if (CancellationToken.IsCancellationRequested)
                return;

            try
            {
                await Task.Delay(interval, CancellationToken);
            }
            catch (TaskCanceledException)
            {
                return;
            }

            await action();

            if (CancellationToken.IsCancellationRequested)
                return;

            await DelayRunAndQueueAnother();
        }
    }
}
