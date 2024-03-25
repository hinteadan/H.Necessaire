using System.IO;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Reporting
{
    public interface ImAReportBuilder
    {
        Note[] SupportedFormats { get; }
    }

    public interface ImAReportBuilder<T> : ImAReportBuilder
    {
        Task<OperationResult<Stream>> BuildReport(T reportData);
    }
}
