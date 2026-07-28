using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImAHealthCheckLogReporter : IStringIdentity
    {
        Task<OperationResult> ReportLogAsOfNow();
    }
}
