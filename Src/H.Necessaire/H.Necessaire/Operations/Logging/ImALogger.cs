using System;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImALogger
    {
        LogConfig LogConfig { get; }
        string Application { get; }
        string Component { get; }

        ImALogger ConfigWith(LogConfig logConfig);

        Task<ImALogger> Log(LogEntry logEntry);

        Task<ImALogger> Log(LogEntryLevel logType, string message, Exception ex = null, object payload = null, params Note[] notes);

        Task<ImALogger> LogTrace(string message, object payload = null, params Note[] notes);

        Task<ImALogger> LogInfo(string message, object payload = null, params Note[] notes);

        Task<ImALogger> LogDebug(string message, object payload = null, params Note[] notes);

        Task<ImALogger> LogWarn(string message, object payload = null, params Note[] notes);

        Task<ImALogger> LogError(string message, object payload = null, params Note[] notes);
        Task<ImALogger> LogError(string message, Exception ex, object payload = null, params Note[] notes);
        Task<ImALogger> LogError(Exception ex, object payload = null, params Note[] notes);

        Task<ImALogger> LogCritical(string message, object payload = null, params Note[] notes);
        Task<ImALogger> LogCritical(string message, Exception ex, object payload = null, params Note[] notes);
        Task<ImALogger> LogCritical(Exception ex, object payload = null, params Note[] notes);
    }
}
