using System.IO;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Reporting.Abstracts
{
    public abstract class ReportBuilderBase : ImAReportBuilder
    {
        readonly Note[] supportedFormats;
        protected ReportBuilderBase(params Note[] supportedFormats)
        {
            this.supportedFormats = supportedFormats;
        }

        public Note[] SupportedFormats => supportedFormats;
    }

    public abstract class ReportBuilderBase<T> : ReportBuilderBase, ImAReportBuilder<T>
    {
        protected ReportBuilderBase(params Note[] supportedFormats) : base(supportedFormats) { }

        public abstract Task<OperationResult<Stream>> BuildReport(T reportData);
    }
}
