using System;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public static class HSafe
    {
        public static OperationResult Run(Action action, string tag = null)
        {
            if (action is null)
                return OperationResult.Fail("Action to run not specified");

            OperationResult result = OperationResult.Fail("Not yet started");

            new Action(() =>
            {
                action.Invoke();
                result = OperationResult.Win();

            }).TryOrFailWithGrace(onFail: ex =>
            {
                result = OperationResult.Fail(ex, $"Error occured while trying to {tag.IfEmpty("run action")}. Reason: {(ex?.Message).IfEmpty("Unknown")}.");
            });

            return result;
        }
        public static OperationResult Run<T1>(Action<T1> action, T1 x1, string tag = null)
            => Run(() => action(x1), tag);
        public static OperationResult Run<T1, T2>(Action<T1, T2> action, T1 x1, T2 x2, string tag = null)
            => Run(() => action(x1, x2), tag);
        public static OperationResult Run<T1, T2, T3>(Action<T1, T2, T3> action, T1 x1, T2 x2, T3 x3, string tag = null)
            => Run(() => action(x1, x2, x3), tag);
        public static OperationResult Run<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 x1, T2 x2, T3 x3, T4 x4, string tag = null)
            => Run(() => action(x1, x2, x3, x4), tag);
        public static OperationResult Run<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, T1 x1, T2 x2, T3 x3, T4 x4, T5 x5, string tag = null)
            => Run(() => action(x1, x2, x3, x4, x5), tag);
        public static OperationResult Run<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action, T1 x1, T2 x2, T3 x3, T4 x4, T5 x5, T6 x6, string tag = null)
            => Run(() => action(x1, x2, x3, x4, x5, x6), tag);

        public static OperationResult<TResult> Run<TResult>(Func<TResult> action, string tag = null)
        {
            if (action is null)
                return OperationResult.Fail("Action to run not specified").WithoutPayload<TResult>();

            OperationResult<TResult> result = OperationResult.Fail("Not yet started").WithoutPayload<TResult>();

            new Action(() =>
            {
                TResult payload = action.Invoke();
                result = OperationResult.Win().WithPayload(payload);

            }).TryOrFailWithGrace(onFail: ex =>
            {
                result = OperationResult.Fail(ex, $"Error occured while trying to {tag.IfEmpty("run action")}. Reason: {(ex?.Message).IfEmpty("Unknown")}.").WithoutPayload<TResult>();
            });

            return result;
        }
        public static OperationResult<TResult> Run<T1, TResult>(Func<T1, TResult> action, T1 x1, string tag = null)
            => Run(() => action(x1), tag);
        public static OperationResult<TResult> Run<T1, T2, TResult>(Func<T1, T2, TResult> action, T1 x1, T2 x2, string tag = null)
            => Run(() => action(x1, x2), tag);
        public static OperationResult<TResult> Run<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> action, T1 x1, T2 x2, T3 x3, string tag = null)
            => Run(() => action(x1, x2, x3), tag);
        public static OperationResult<TResult> Run<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> action, T1 x1, T2 x2, T3 x3, T4 x4, string tag = null)
            => Run(() => action(x1, x2, x3, x4), tag);
        public static OperationResult<TResult> Run<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> action, T1 x1, T2 x2, T3 x3, T4 x4, T5 x5, string tag = null)
            => Run(() => action(x1, x2, x3, x4, x5), tag);
        public static OperationResult<TResult> Run<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> action, T1 x1, T2 x2, T3 x3, T4 x4, T5 x5, T6 x6, string tag = null)
            => Run(() => action(x1, x2, x3, x4, x5, x6), tag);


        public static async Task<OperationResult> Run(Func<Task> action, string tag = null)
        {
            if (action is null)
                return OperationResult.Fail("Action to run not specified");

            OperationResult result = OperationResult.Fail("Not yet started");

            await new Func<Task>(async () =>
            {
                await action.Invoke();
                result = OperationResult.Win();

            }).TryOrFailWithGrace(onFail: ex =>
            {
                result = OperationResult.Fail(ex, $"Error occured while trying to {tag.IfEmpty("run action")}. Reason: {(ex?.Message).IfEmpty("Unknown")}.");
            });

            return result;
        }
        public static Task<OperationResult> Run<T1>(Func<T1, Task> action, T1 x1, string tag = null)
            => Run(() => action(x1), tag);
        public static Task<OperationResult> Run<T1, T2>(Func<T1, T2, Task> action, T1 x1, T2 x2, string tag = null)
            => Run(() => action(x1, x2), tag);
        public static Task<OperationResult> Run<T1, T2, T3>(Func<T1, T2, T3, Task> action, T1 x1, T2 x2, T3 x3, string tag = null)
            => Run(() => action(x1, x2, x3), tag);
        public static Task<OperationResult> Run<T1, T2, T3, T4>(Func<T1, T2, T3, T4, Task> action, T1 x1, T2 x2, T3 x3, T4 x4, string tag = null)
            => Run(() => action(x1, x2, x3, x4), tag);
        public static Task<OperationResult> Run<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, Task> action, T1 x1, T2 x2, T3 x3, T4 x4, T5 x5, string tag = null)
            => Run(() => action(x1, x2, x3, x4, x5), tag);
        public static Task<OperationResult> Run<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, Task> action, T1 x1, T2 x2, T3 x3, T4 x4, T5 x5, T6 x6, string tag = null)
            => Run(() => action(x1, x2, x3, x4, x5, x6), tag);

        public static async Task<OperationResult<TResult>> Run<TResult>(Func<Task<TResult>> action, string tag = null)
        {
            if (action is null)
                return OperationResult.Fail("Action to run not specified").WithoutPayload<TResult>();

            OperationResult<TResult> result = OperationResult.Fail("Not yet started").WithoutPayload<TResult>();

            await new Func<Task>(async () =>
            {
                TResult payload = await action.Invoke();
                result = OperationResult.Win().WithPayload(payload);

            }).TryOrFailWithGrace(onFail: ex =>
            {
                result = OperationResult.Fail(ex, $"Error occured while trying to {tag.IfEmpty("run action")}. Reason: {(ex?.Message).IfEmpty("Unknown")}.").WithoutPayload<TResult>();
            });

            return result;
        }
        public static Task<OperationResult<TResult>> Run<T1, TResult>(Func<T1, Task<TResult>> action, T1 x1, string tag = null)
            => Run(() => action(x1), tag);
        public static Task<OperationResult<TResult>> Run<T1, T2, TResult>(Func<T1, T2, Task<TResult>> action, T1 x1, T2 x2, string tag = null)
            => Run(() => action(x1, x2), tag);
        public static Task<OperationResult<TResult>> Run<T1, T2, T3, TResult>(Func<T1, T2, T3, Task<TResult>> action, T1 x1, T2 x2, T3 x3, string tag = null)
            => Run(() => action(x1, x2, x3), tag);
        public static Task<OperationResult<TResult>> Run<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, Task<TResult>> action, T1 x1, T2 x2, T3 x3, T4 x4, string tag = null)
            => Run(() => action(x1, x2, x3, x4), tag);
        public static Task<OperationResult<TResult>> Run<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, Task<TResult>> action, T1 x1, T2 x2, T3 x3, T4 x4, T5 x5, string tag = null)
            => Run(() => action(x1, x2, x3, x4, x5), tag);
        public static Task<OperationResult<TResult>> Run<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, Task<TResult>> action, T1 x1, T2 x2, T3 x3, T4 x4, T5 x5, T6 x6, string tag = null)
            => Run(() => action(x1, x2, x3, x4, x5, x6), tag);

        public static async Task RunWithRetry(Func<Task> action, Func<bool> successCondition = null, int maxRetries = 3, double timeoutScaleInMs = .27)
        {
            int retryCount = 0;

            while (retryCount <= maxRetries)
            {
                var runResult = await Run(action);

                if (!runResult || !(successCondition?.Invoke() ?? true))
                {
                    retryCount++;
                    await Task.Delay(TimeSpan.FromSeconds(timeoutScaleInMs + retryCount * timeoutScaleInMs));
                    continue;
                }

                return;
            }
        }

        public static async Task<T> RunWithRetry<T>(Func<Task<T>> action, Func<T, bool> successCondition = null, int maxRetries = 3, double timeoutScaleInMs = .27)
        {
            T result = default(T);

            await RunWithRetry(async () => result = await action.Invoke(), () => successCondition?.Invoke(result) ?? true, maxRetries, timeoutScaleInMs);

            return result;
        }
    }
}
