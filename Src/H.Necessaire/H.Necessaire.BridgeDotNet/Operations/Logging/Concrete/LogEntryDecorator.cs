using Bridge;
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
                    x.StackTrace = logEntry.Level >= minimumLevelForStackTrace ? Script.Eval<string>("try { new Error().stack } catch { null }") : null;
                    x.ScopeID = CallContext<Guid?>.GetData(CallContextKey.LoggingScopeID) ?? x.ScopeID;
                    x.OperationContext = CallContext<OperationContext>.GetData(CallContextKey.OperationContext);
                });
        }
    }

    public static class MethodName
    {
        public static string GetCurrentName(string methodName = null) => Script.Eval<string>("try { arguments.callee.name } catch { null }");
    }
}
