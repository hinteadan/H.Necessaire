using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImAReleaseVersionProvider
    {
        Task<ReleaseVersion[]> GetAllReleases();
    }
}
