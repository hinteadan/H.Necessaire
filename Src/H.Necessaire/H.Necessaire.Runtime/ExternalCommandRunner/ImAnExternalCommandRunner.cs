using System.Threading;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.ExternalCommandRunner
{
    public interface ImAnExternalCommandRunner
    {
        Task<OperationResult<ExternalCommandRunContext>> RunCmd(params Note[] args);
        Task<OperationResult<ExternalCommandRunContext>> RunCmd(CancellationToken cancellationToken, params Note[] args);
        Task<OperationResult<ExternalCommandRunContext>> Run(params Note[] args);
        Task<OperationResult<ExternalCommandRunContext>> Run(CancellationToken cancellationToken, params Note[] args);
    }
}