using System;
using System.Diagnostics;

namespace H.Necessaire
{
    public class TimeMeasurement : IDisposable
    {
        readonly Stopwatch stopwatch = new Stopwatch();
        readonly ScopedRunner scopedRunner;
        readonly Action<TimeSpan> onDone;

        public TimeMeasurement(Action<TimeSpan> onDone)
        {
            this.onDone = onDone;
            scopedRunner
                = new ScopedRunner(
                    () => stopwatch.Start(),
                    () => { stopwatch.Stop(); this.onDone?.Invoke(stopwatch.Elapsed); }
                );
        }

        public void Dispose()
        {
            scopedRunner.Dispose();
        }
    }
}
