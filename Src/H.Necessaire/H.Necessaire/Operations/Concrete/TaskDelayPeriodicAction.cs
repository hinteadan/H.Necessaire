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

        public void Start(TimeSpan interval, Func<Task> action)
            => StartDelayed(TimeSpan.Zero, interval, action);

        public async void StartDelayed(TimeSpan delay, TimeSpan interval, Func<Task> action)
        {
            if (action is null)
                return;

            CancellationToken cnclToken;
            lock (isStartedLocker)
            {
                if (isStarted)
                    return;

                isStarted = true;

                cnclToken = cancellationTokenSource.Token;
            }

            this.action = action;
            this.interval = interval <= TimeSpan.Zero ? TimeSpan.Zero : interval;

            try
            {
                await Task.Delay(delay, cnclToken);
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (TaskCanceledException)
            {
                return;
            }

            await HSafe.Run(action);

            DelayRunAndQueueAnother(cnclToken).DontWait();
        }

        public void Stop()
        {
            lock (isStartedLocker)
            {
                if (!isStarted)
                    return;

                isStarted = false;
            }

            var prevToken = Interlocked.Exchange(ref cancellationTokenSource, new CancellationTokenSource());

            prevToken.Cancel();
            prevToken.Dispose();
        }

        async Task DelayRunAndQueueAnother(CancellationToken cnclToken)
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

            if (cnclToken.IsCancellationRequested)
                return;

            try
            {
                await Task.Delay(interval, cnclToken);
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (TaskCanceledException)
            {
                return;
            }

            await HSafe.Run(action);

            if (cnclToken.IsCancellationRequested)
                return;

            DelayRunAndQueueAnother(cnclToken).DontWait();
        }
    }
}
