using System;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public static class ExecutionUtilities
    {
        public static void TryAFewTimesOrFailWithGrace(Action doThis, int numberOfTimes = 1, Action<Exception> onFail = null, Action<Exception> onRetry = null, int millisecondsToSleepBetweenRetries = 500)
        {
            if (numberOfTimes <= 0)
                return;

            try
            {
                doThis.Invoke();
            }
            catch (Exception ex)
            {
                numberOfTimes--;
                if (numberOfTimes == 0)
                {
                    try { onFail?.Invoke(ex); } catch (Exception) { }
                    return;
                }
                else
                    try { onRetry?.Invoke(ex); } catch (Exception) { }

                System.Threading.Thread.Sleep(millisecondsToSleepBetweenRetries > 10000 ? 10000 : millisecondsToSleepBetweenRetries);

                TryAFewTimesOrFailWithGrace(doThis, numberOfTimes, onFail, onRetry, (int)(millisecondsToSleepBetweenRetries * 1.3));
            }
        }

        public static async Task TryAFewTimesOrFailWithGrace(Func<Task> doThis, int numberOfTimes = 1, Action<Exception> onFail = null, Action<Exception> onRetry = null, int millisecondsToSleepBetweenRetries = 500)
        {
            if (numberOfTimes <= 0)
                return;

            try
            {
                await doThis.Invoke();
            }
            catch (Exception ex)
            {
                numberOfTimes--;
                if (numberOfTimes == 0)
                {
                    try { onFail?.Invoke(ex); } catch (Exception) { }
                    return;
                }
                else
                    try { onRetry?.Invoke(ex); } catch (Exception) { }

                System.Threading.Thread.Sleep(millisecondsToSleepBetweenRetries > 10000 ? 10000 : millisecondsToSleepBetweenRetries);

                await TryAFewTimesOrFailWithGrace(doThis, numberOfTimes, onFail, onRetry, (int)(millisecondsToSleepBetweenRetries * 1.3));
            }

        }

        public static async Task TryAFewTimesOrFailWithGrace(Func<Task> doThis, int numberOfTimes = 1, Func<Exception, Task> onFail = null, Func<Exception, Task> onRetry = null, int millisecondsToSleepBetweenRetries = 500)
        {
            if (numberOfTimes <= 0)
                return;

            try
            {
                await doThis.Invoke();
            }
            catch (Exception ex)
            {
                numberOfTimes--;
                if (numberOfTimes == 0)
                {
                    try { await onFail?.Invoke(ex); } catch (Exception) { }
                    return;
                }
                else
                    try { await onRetry?.Invoke(ex); } catch (Exception) { }

                System.Threading.Thread.Sleep(millisecondsToSleepBetweenRetries > 10000 ? 10000 : millisecondsToSleepBetweenRetries);

                await TryAFewTimesOrFailWithGrace(doThis, numberOfTimes, onFail, onRetry, (int)(millisecondsToSleepBetweenRetries * 1.3));
            }

        }

        public static void TryOrFailWithGrace(this Action doThis, int numberOfTimes = 1, Action<Exception> onFail = null, Action<Exception> onRetry = null, int millisecondsToSleepBetweenRetries = 500)
            => TryAFewTimesOrFailWithGrace(doThis, numberOfTimes, onFail, onRetry, millisecondsToSleepBetweenRetries);

        public static Task TryOrFailWithGrace(this Func<Task> doThis, int numberOfTimes = 1, Action<Exception> onFail = null, Action<Exception> onRetry = null, int millisecondsToSleepBetweenRetries = 500)
            => TryAFewTimesOrFailWithGrace(doThis, numberOfTimes, onFail, onRetry, millisecondsToSleepBetweenRetries);

        public static Task TryOrFailWithGrace(this Func<Task> doThis, int numberOfTimes = 1, Func<Exception, Task> onFail = null, Func<Exception, Task> onRetry = null, int millisecondsToSleepBetweenRetries = 500)
            => TryAFewTimesOrFailWithGrace(doThis, numberOfTimes, onFail, onRetry, millisecondsToSleepBetweenRetries);
    }
}
