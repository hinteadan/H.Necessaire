using System.Threading.Tasks;

namespace H.Necessaire.Runtime.ExternalCommandRunner
{
    public interface ImAnExternalCommandRunner
    {
        Task<OperationResult<ExternalCommandRunContext>> RunCmd(params Note[] args);
        Task<OperationResult<ExternalCommandRunContext>> Run(params Note[] args);
    }
}