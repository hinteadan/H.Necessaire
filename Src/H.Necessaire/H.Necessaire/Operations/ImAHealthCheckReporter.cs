using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImAHealthCheckReporter : IStringIdentity
    {
        Task<OperationResult> ReportAliveAndHealthyAsOfNow();
        Task<OperationResult> ReportUnhealthyAsOfNow();
    }
}
