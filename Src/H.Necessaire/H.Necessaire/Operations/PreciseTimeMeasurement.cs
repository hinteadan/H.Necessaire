using System;
using System.Diagnostics;

namespace H.Necessaire
{
    public class PreciseTimeMeasurement : IDisposable
    {
        readonly ScopedRunner scopedRunner;
        readonly Action<TimeSpan> onDone;
        long startTime;

        public PreciseTimeMeasurement(Action<TimeSpan> onDone)
        {
            this.onDone = onDone;
            scopedRunner
                = new ScopedRunner(
                    () => startTime = Stopwatch.GetTimestamp(),
                    () => { TimeSpan duration = TimeSpan.FromTicks(Stopwatch.GetTimestamp() - startTime); this.onDone?.Invoke(duration); }
                );
        }

        public void Dispose()
        {
            scopedRunner.Dispose();
        }
    }
}
