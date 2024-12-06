using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace H.Necessaire
{
    [DataContract]
    public class LogEntry : IGuidIdentity, ImSyncable
    {
        [DataMember] public Guid ID { get; set; } = Guid.NewGuid();
        [DataMember] public LogEntryLevel Level { get; set; } = LogEntryLevel.Info;
        [DataMember] public Guid ScopeID { get; set; } = Guid.NewGuid();
        [DataMember] public OperationContext OperationContext { get; set; } = null;
        [DataMember] public DateTime HappenedAt { get; set; } = DateTime.UtcNow;
        [DataMember] public string Message { get; set; } = null;
        [DataMember] public string Method { get; set; } = MethodName.GetCurrentName();
        [DataMember] public string StackTrace { get; set; } = null;
        [DataMember] public string Component { get; set; } = null;
        [DataMember] public string Application { get; set; } = "H.Necessaire";
        [DataMember] public Version AppVersion { get; set; } = null;
        Exception exception = null;
        public Exception Exception { get => exception; set => exception = value.And(x => Exceptions = x.Flatten()?.Select(ex => new LogEntryExceptionInfo(ex)).ToArrayNullIfEmpty()); }
        [DataMember] public LogEntryExceptionInfo[] Exceptions { get; set; }
        [DataMember] public object Payload { get; set; } = null;
        [DataMember] public Note[] Notes { get; set; }

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
