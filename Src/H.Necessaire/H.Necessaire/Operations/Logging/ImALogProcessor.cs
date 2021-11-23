using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImALogProcessor
    {
        LoggerPriority GetPriority();
        ImALogProcessor ConfigWith(LogConfig logConfig);
        Task<bool> IsEligibleFor(LogEntry logEntry);
        Task<OperationResult<LogEntry>> Process(LogEntry logEntry);
    }
}
