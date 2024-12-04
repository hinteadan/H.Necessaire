using System;
using System.Text;
using System.Threading;

namespace H.Necessaire.Runtime.ExternalCommandRunner
{
    public class ExternalCommandRunContext
    {
        public StringBuilder OutputData { get; } = new StringBuilder();
        public StringBuilder ErrorData { get; } = new StringBuilder();
        public bool IsUserInputExpected { get; set; } = false;
        public bool IsOutputCaptured { get; set; } = false;
        /// <summary>
        /// Obviously, only has effect if IsOutputCaptured is true, otherwise the output is sent to the std. output by the underlying command, without our control.
        /// </summary>
        public bool IsOutputPrinted { get; set; } = true;
        public CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource();
        public bool HasErrors => ErrorData.Length > 0;
        public bool HasData => OutputData.Length > 0;
        public static ExternalCommandRunContext GetCurrent() => ExternalCommandRunScope.GetCurrentContext();
    }

    public static class ExternalCommandRunContextExtensions
    {
        public static IDisposable Scope(this ExternalCommandRunContext context)
        {
            if (context is null)
                OperationResult.Fail("Context cannot be null").ThrowOnFail();

            return new ExternalCommandRunScope(context);
        }
    }
}
