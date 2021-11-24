using H.Necessaire.Runtime.Versioning;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.UseCases.Concrete
{
    internal class VersionUseCase : UseCaseBase, ImAVersionUseCase
    {
        VersionProvider versionProvider;
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);
            this.versionProvider = dependencyProvider.Get<VersionProvider>();
        }

        public Task<Version> GetCurrentVersion()
        {
            return versionProvider.GetCurrentVersion();
        }

        public async Task<string> GetCurrentVersionAsString()
        {
            return (await versionProvider.GetCurrentVersion()).ToString();
        }
    }
}
