using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImALogBrowser
    {
        Task<OperationResult<LogEntry[]>> Browse(LogFilter filter);

        Task<OperationResult<Page<LogEntry>>> LoadPage(LogFilter filter);

        Task<OperationResult<IDisposableEnumerable<LogEntry>>> Stream(LogFilter filter);

        Task<OperationResult<IDisposableEnumerable<LogEntry>>> StreamAll();
    }
}
