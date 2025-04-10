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
                result = OperationResult.Fail(ex, $"Error occured while trying to run {tag.IfEmpty("action")}. Reason: {(ex?.Message).IfEmpty("Unknown")}.");
            });

            return result;
        }
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
                result = OperationResult.Fail(ex, $"Error occured while trying to run {tag.IfEmpty("action")}. Reason: {(ex?.Message).IfEmpty("Unknown")}.").WithoutPayload<TResult>();
            });

            return result;
        }

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
                result = OperationResult.Fail(ex, $"Error occured while trying to run {tag.IfEmpty("action")}. Reason: {(ex?.Message).IfEmpty("Unknown")}.");
            });

            return result;
        }
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
                result = OperationResult.Fail(ex, $"Error occured while trying to run {tag.IfEmpty("action")}. Reason: {(ex?.Message).IfEmpty("Unknown")}.").WithoutPayload<TResult>();
            });

            return result;
        }
    }
}
