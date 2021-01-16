using Bridge.Html5;
using System;
using System.Threading.Tasks;

namespace H.Necessaire.Operations.Concrete
{
    public class JsPeriodicAction : ImAPeriodicAction
    {
        private int timeoutId = -1;
        private bool isStarted = false;

        public async void StartDelayed(TimeSpan delay, TimeSpan interval, Func<Task> action)
        {
            if (isStarted)
                return;

            isStarted = true;

            if (delay > TimeSpan.Zero)
                await Task.Delay((int)delay.TotalMilliseconds);

            if (!isStarted)
                return;

            await ExecuteActionAndQueueAnother(action, interval);
        }

        public void Start(TimeSpan interval, Func<Task> action)
            => StartDelayed(TimeSpan.Zero, interval, action);

        public void Start(TimeSpan interval, Action action)
            => Start(interval, () => { action?.Invoke(); return Task.FromResult(true); });

        public void StartDelayed(TimeSpan delay, TimeSpan interval, Action action)
            => StartDelayed(delay, interval, () => { action?.Invoke(); return Task.FromResult(true); });

        public void Stop()
        {
            if (!isStarted)
                return;

            Window.ClearTimeout(timeoutId);

            isStarted = false;
        }

        private async Task ExecuteActionAndQueueAnother(Func<Task> action, TimeSpan interval)
        {
            await action?.Invoke();

            if (!isStarted)
                return;

            timeoutId = Window.SetTimeout(async () => await ExecuteActionAndQueueAnother(action, interval), (int)interval.TotalMilliseconds);
        }
    }
}
