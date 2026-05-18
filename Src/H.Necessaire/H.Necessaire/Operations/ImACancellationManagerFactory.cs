using System.Threading;

namespace H.Necessaire
{
    public interface ImACancellationManagerFactory
    {
        ImACancellationManager New();
        ImACancellationManager New(params CancellationToken[] linkedTokens);
    }
}
