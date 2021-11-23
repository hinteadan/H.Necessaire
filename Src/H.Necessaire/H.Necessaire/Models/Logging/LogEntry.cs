using System;
using System.Linq;

namespace H.Necessaire
{
    public class LogEntry : IGuidIdentity, ImSyncable
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public LogEntryLevel Level { get; set; } = LogEntryLevel.Info;
        public Guid ScopeID { get; set; } = Guid.NewGuid();
        public OperationContext OperationContext { get; set; } = null;
        public DateTime HappenedAt { get; set; } = DateTime.UtcNow;
        public string Message { get; set; } = null;
        public string Method { get; set; } = MethodName.GetCurrentName();
        public string StackTrace { get; set; } = null;
        public string Component { get; set; } = null;
        public string Application { get; set; } = "H.Necessaire";
        public Exception Exception { get; set; } = null;
        public object Payload { get; set; } = null;
        public Note[] Notes { get; set; }

        public override string ToString()
        {
            return
                string.Join(" | ", new string[] {
                    $"{HappenedAt}", $"{Level}", Application, Component, Method, Message
                }.Where(x => !string.IsNullOrWhiteSpace(x)))
                + (string.IsNullOrWhiteSpace(StackTrace) ? string.Empty : $"{Environment.NewLine}{Environment.NewLine}{StackTrace}");
        }

        public static LogEntry Build(LogEntryLevel level, string message, Exception ex = null, object payload = null, params Note[] notes)
        {
            return
                new LogEntry
                {
                    Level = level,
                    Message = !string.IsNullOrWhiteSpace(message) ? message : ex?.Message,
                    Exception = ex,
                    Payload = payload,
                    Notes = notes?.Any() == true ? notes : null,
                };
        }
    }
}
