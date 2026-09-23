using System;
using System.Threading;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public static partial class HSafe
    {
        public static async Task<OperationResult<TimeSpan?>> RunTimeboxed(TimeSpan timeoutAfter, Action<CancellationToken> actionToRun)
            => await RunTimeboxed(timeoutAfter, actionToRun is null ? null as Func<CancellationToken, Task> : t => { actionToRun(t); return Task.CompletedTask; });

        public static async Task<OperationResult<TimeSpan?>> RunTimeboxed(TimeSpan timeoutAfter, Func<CancellationToken, Task> actionToRun)
        {
            if (actionToRun is null)
                return $"{nameof(actionToRun)} is not defined";

            using (var timeoutCts = new CancellationTokenSource(timeoutAfter))
            {
                OperationResult<TimeSpan?> executionResult = "Not yet started";
                Task executionTask = Task.Run(async () =>
                {
                    executionResult = await HSafe.Run(async () =>
                    {
                        TimeSpan? executionDuration = null;
                        using (new PreciseTimeMeasurement(x => executionDuration = x))
                        {
                            await actionToRun.Invoke(timeoutCts.Token);
                        }
                        return executionDuration;
                    });
                });
                Task timeoutTask = HSafe.Run(async () => await Task.Delay(Timeout.Infinite, timeoutCts.Token));
                Task completedTask = await Task.WhenAny(executionTask, timeoutTask);

                OperationResult<TimeSpan?> result
                    = completedTask == timeoutTask
                    ? $"Execution timed out after {timeoutAfter}"
                    : executionResult
                    ;

                return result;
            }
        }
    }
}
