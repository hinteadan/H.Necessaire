using System;

namespace H.Necessaire
{
    public class ScopedRunner : IDisposable
    {
        readonly Action onStart;
        readonly Action onStop;

        public ScopedRunner(Action onStart, Action onStop)
        {
            this.onStart = onStart;
            this.onStop = onStop;

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
