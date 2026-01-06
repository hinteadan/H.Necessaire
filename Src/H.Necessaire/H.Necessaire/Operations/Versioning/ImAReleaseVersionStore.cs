using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImAReleaseVersionStore
    {
        Task<OperationResult> Save(params ReleaseVersion[] releaseVersions);
    }
}
