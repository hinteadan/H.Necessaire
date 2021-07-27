using System.Threading.Tasks;

namespace H.Necessaire.Runtime
{
    public interface ImAPingUseCase : ImAUseCase
    {
        Task<string> Pong();
        Task<string> SecuredPong();
    }
}
