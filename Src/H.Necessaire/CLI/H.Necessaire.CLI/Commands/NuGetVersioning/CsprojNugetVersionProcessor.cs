using H.Necessaire.CLI.Commands.NuGetVersioning.Models;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.CLI.Commands.NuGetVersioning
{
    class CsprojNugetVersionProcessor : ImADependency
    {
        #region Construct
        CsprojParser csprojParser = null;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            csprojParser = dependencyProvider.Get<CsprojParser>();
        }
        #endregion

        public async Task<OperationResult<NuGetIdentifier[]>> GetAllLatestNuGets()
        {
            CsprojInfo[] allCsProjs = (await csprojParser.GetAllCsProjs()).ThrowOnFailOrReturn();

            return
                allCsProjs
                .SelectMany(x => x.NuGets)
                .OrderByDescending(x => x.VersionNumber, VersionNumber.Comparer)
                .GroupBy(x => x.ID)
                .Select(x => x.First())
                .ToArray()
                .ToWinResult()
                ;
        }
    }
}
