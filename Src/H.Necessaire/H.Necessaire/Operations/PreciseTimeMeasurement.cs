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
                    () => this.onDone?.Invoke(CalculateOSSafeTimeSpan(startTime, Stopwatch.GetTimestamp()))
                );
        }

        public void Dispose()
        {
            scopedRunner.Dispose();
        }

        static TimeSpan CalculateOSSafeTimeSpan(long startTime, long endTime)
            => TimeSpan.FromTicks(
                ((endTime - startTime) * TimeSpan.TicksPerSecond) / Stopwatch.Frequency
            );
    }
}
