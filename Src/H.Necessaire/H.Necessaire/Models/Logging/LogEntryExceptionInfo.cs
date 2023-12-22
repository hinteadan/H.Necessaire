using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace H.Necessaire
{
    public class LogEntryExceptionInfo
    {
        readonly Dictionary<string, string> data = new Dictionary<string, string>();
        public LogEntryExceptionInfo(Exception exception)
        {
            if (exception == null)
                return;

            if ((exception.Data?.Count ?? 0) > 0)
            {
                foreach (KeyValuePair<object, object> entry in exception.Data)
                    data.Add(entry.Key?.ToString(), entry.Value?.ToString());
            }

            Message = exception.Message;
            StackTrace = exception.StackTrace;
            HResult = exception.HResult;

            PropertyInfo[] props
                = exception
                .GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.CanRead && p.Name.NotIn("Data", "Message", "StackTrace", "HResult", "InnerException", "InnerExceptions"))
                .ToArray();

            Notes
                = props
                .Select(prop =>
                    prop
                    .GetValue(exception)
                    ?.ToString()
                    .NullIfEmpty()
                    .NoteAs(prop.Name)
                )
                .ToNoNullsArray()
                ?.Select(x => x.Value)
                .ToArrayNullIfEmpty()
                ;
        }

        public IDictionary<string, string> Data => data;
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public int HResult { get; set; }

        public Note[] Notes { get; set; }
    }
}
