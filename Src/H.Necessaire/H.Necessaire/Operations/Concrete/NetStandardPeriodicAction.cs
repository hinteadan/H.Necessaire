using System;
using System.Threading.Tasks;
using System.Timers;

namespace H.Necessaire.Operations.Concrete
{
    class NetStandardPeriodicAction : ImAPeriodicAction, IDisposable
    {
        private bool isStarted = false;
        private readonly Timer timer = new Timer { AutoReset = false };
        private Func<Task> action;

        public async void StartDelayed(TimeSpan delay, TimeSpan interval, Func<Task> action)
        {
            if (isStarted)
                return;

            isStarted = true;

            this.action = action;

            timer.Interval = interval.TotalMilliseconds;
            timer.Elapsed += Timer_Elapsed;

            if (delay > TimeSpan.Zero)
                await Task.Delay((int)delay.TotalMilliseconds);

            if (!isStarted)
                return;

            await action();

            timer.Start();
        }

        public void Start(TimeSpan interval, Func<Task> action)
            => StartDelayed(TimeSpan.Zero, interval, action);

        public void Stop()
        {
            if (!isStarted)
                return;

            timer.Elapsed -= Timer_Elapsed;
            timer.Stop();
            isStarted = false;
        }

        public void Dispose()
        {
            Stop();
            timer.Dispose();
        }

        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            await action();
            timer.Start();
        }
    }
}
