using System.Threading.Tasks;

namespace H.Necessaire.Runtime
{
    public interface ImAUseCaseContextProvider
    {
        Task<UseCaseContext> GetCurrentContext();
    }
}
