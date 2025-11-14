using System.Threading;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface IHRun
    {
        Task<OperationResult> Run(CancellationToken? cancellationToken = null);
    }
}
