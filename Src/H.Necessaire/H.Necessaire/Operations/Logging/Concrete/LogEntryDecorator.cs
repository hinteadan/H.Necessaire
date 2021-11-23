using System;

namespace H.Necessaire
{
    public class LogEntryDecorator
    {
        protected string Application { get; }
        protected string Component { get; }
        public LogEntryDecorator(string component = null, string application = "H.Necessaire")
        {
            this.Application = application;
            this.Component = component;
        }

        public LogEntry DecorateLogEntry(LogEntry logEntry, LogEntryLevel minimumLevelForStackTrace = LogEntryLevel.Error)
        {
            return
                logEntry
                .And(x =>
                {
                    x.Application = Application;
                    x.Component = Component;
                    x.ScopeID = CallContext<Guid?>.GetData(CallContextKey.LoggingScopeID) ?? x.ScopeID;
                    x.OperationContext = CallContext<OperationContext>.GetData(CallContextKey.OperationContext);
                    x.StackTrace = x.Exception?.StackTrace;
                })
                .And(x =>
                {
                    if (!string.IsNullOrWhiteSpace(x.StackTrace) || logEntry.Level < minimumLevelForStackTrace)
                        return;

                    new Action(() =>
                    {
                        string stackTrace = Environment.StackTrace;
                        x.StackTrace = stackTrace?.Substring(stackTrace.IndexOf(Environment.NewLine) + Environment.NewLine.Length);
                    })
                    .TryOrFailWithGrace();
                });
        }
    }

    public static class MethodName
    {
        static readonly string[] methodNamesToIgnore = new string[] { "Method" };

        public static string GetCurrentName([System.Runtime.CompilerServices.CallerMemberName] string methodName = null)
            => string.IsNullOrWhiteSpace(methodName)
            || methodName.In(
                methodNamesToIgnore,
                (a, b) => string.Equals(a, b, StringComparison.InvariantCultureIgnoreCase)
            )
            ? null
            : methodName
            ;
    }
}
