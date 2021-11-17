using System.Threading;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImADaemon
    {
        Task Start(CancellationToken? cancellationToken = null);

        Task Stop(CancellationToken? cancellationToken = null);
    }
}
