using System.Threading.Tasks;

namespace H.Necessaire.Runtime
{
    public interface ImAVersionUseCase : ImAUseCase
    {
        Task<Version> GetCurrentVersion();
        Task<string> GetCurrentVersionAsString();
    }
}
