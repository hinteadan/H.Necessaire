using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImAVersionProvider
    {
        Task<Version> GetCurrentVersion();
    }
}
