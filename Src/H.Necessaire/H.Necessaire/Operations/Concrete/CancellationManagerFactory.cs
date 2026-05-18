using System.Threading;

namespace H.Necessaire.Operations.Concrete
{
    internal class CancellationManagerFactory : ImACancellationManagerFactory
    {
        public ImACancellationManager New()
            => new CancellationManager();

        public ImACancellationManager New(params CancellationToken[] linkedTokens)
            => new CancellationManager(linkedTokens);
    }
}
