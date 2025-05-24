using System.Diagnostics;

namespace H.Necessaire.Runtime.MAUI.Core
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
                    () => { TimeSpan duration = Stopwatch.GetElapsedTime(startTime); this.onDone?.Invoke(duration); }
                );
        }

        public void Dispose()
        {
            scopedRunner.Dispose();
        }
    }
}
