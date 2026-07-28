using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImATimeboxHealthCheckReporter : IStringIdentity
    {
        Task<OperationResult> ReportStartAsOfNow();
        Task<OperationResult> ReportFailAsOfNow();
        Task<OperationResult> ReportFinishAsOfNow(byte? exitCode = null);
    }
}
