using System;

namespace H.Necessaire
{
    public class ScopedRunner : IDisposable
    {
        public static ScopedRunner Null { get; } = new ScopedRunner(null as Action, null);

        readonly Action onStart;
        readonly Action onStop;

        public ScopedRunner(Action onStart, Action onStop)
        {
            this.onStart = onStart;
            this.onStop = onStop;

            DoStart();
        }

        public ScopedRunner(Action<bool> onStart, Action<bool> onStop)
        {
            this.onStart = () => onStart?.Invoke(true);
            this.onStop = () => onStop?.Invoke(false);

            DoStart();
        }

        public virtual void Dispose()
        {
            DoStop();
        }

        private void DoStart()
        {
            if (onStart == null)
                return;

            onStart.TryOrFailWithGrace();
        }

        private void DoStop()
        {
            if (onStop == null)
                return;

            onStop.TryOrFailWithGrace();
        }
    }
}
