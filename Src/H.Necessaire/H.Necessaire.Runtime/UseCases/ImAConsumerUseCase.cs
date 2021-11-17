using System.Threading.Tasks;

namespace H.Necessaire.Runtime
{
    public interface ImAConsumerUseCase : ImAUseCase
    {
        Task<ConsumerIdentity> CreateOrResurrect();

        Task<ConsumerIdentity> Resurrect();
    }
}
