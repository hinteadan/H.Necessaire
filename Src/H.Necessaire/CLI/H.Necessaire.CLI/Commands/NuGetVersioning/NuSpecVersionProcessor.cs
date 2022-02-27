using H.Necessaire.CLI.Commands.NuGetVersioning.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.CLI.Commands.NuGetVersioning
{
    class NuSpecVersionProcessor : ImADependency
    {
        #region Construct
        NuSpecParser nuSpecParser = new NuSpecParser();
        NuSpecDependencyTreeProcessor nuSpecDependencyTreeProcessor = new NuSpecDependencyTreeProcessor();
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            nuSpecParser = dependencyProvider.Get<NuSpecParser>();
            nuSpecDependencyTreeProcessor = dependencyProvider.Get<NuSpecDependencyTreeProcessor>();
        }
        #endregion

        public async Task<OperationResult<NuSpecInfo[]>> UpdateExternalNuGetVersion(string externalNuGetID, string newVersion, bool updateNuSpecVersion = true)
        {
            NuSpecInfo[] nuSpecs = (await nuSpecParser.GetAllNuSpecs()).ThrowOnFailOrReturn() ?? new NuSpecInfo[0];
            NuSpecInfo[] nuSpecsWithExternalNuGet = nuSpecs.Where(x => externalNuGetID.In(x.Dependencies?.Select(d => d.ID))).ToArray();

            foreach (NuSpecInfo nuSpec in nuSpecsWithExternalNuGet)
            {
                nuSpec.Dependencies.Single(x => x.ID == externalNuGetID).UpdateVersionTo(VersionNumber.Parse(newVersion));
                if (updateNuSpecVersion)
                    await UpdateVersion(nuSpec.ID, x => x.IncrementPatchVersion(), nuSpecs);
            }

            return OperationResult.Win().WithPayload(nuSpecs);
        }

        public Task<OperationResult<NuSpecInfo[]>> IncrementMajorVersion(string nuGetID)
        {
            return UpdateVersion(nuGetID, x => x.IncrementMajorVersion());
        }

        public Task<OperationResult<NuSpecInfo[]>> IncrementMinorVersion(string nuGetID)
        {
            return UpdateVersion(nuGetID, x => x.IncrementMinorVersion());
        }

        public Task<OperationResult<NuSpecInfo[]>> IncrementPatchVersion(string nuGetID)
        {
            return UpdateVersion(nuGetID, x => x.IncrementPatchVersion());
        }

        public Task<OperationResult<NuSpecInfo[]>> IncrementBuildVersion(string nuGetID)
        {
            return UpdateVersion(nuGetID, x => x.IncrementBuildVersion());
        }

        public Task<OperationResult<NuSpecInfo[]>> SetVersionSuffix(string nuGetID, string suffix)
        {
            return UpdateVersion(nuGetID, x => x.SetVersionSuffix(suffix));
        }

        private async Task<OperationResult<NuSpecInfo[]>> UpdateVersion(string nuGetID, Action<NuSpecTree> versionUpdater, NuSpecInfo[] nuSpecsPool = null)
        {
            NuSpecInfo[] nuSpecs = nuSpecsPool ?? (await nuSpecParser.GetAllNuSpecs()).ThrowOnFailOrReturn() ?? new NuSpecInfo[0];

            NuSpecTree depTree = (await nuSpecDependencyTreeProcessor.ProcessDependencyTreeFor(nuSpecs.Single(x => x.ID == nuGetID), nuSpecs)).ThrowOnFailOrReturn();

            versionUpdater(depTree);

            UpdateNuSpecDependencyVersions(nuSpecs);

            return OperationResult.Win().WithPayload(nuSpecs);
        }

        private void UpdateNuSpecDependencyVersions(NuSpecInfo[] nuSpecs)
        {
            foreach (NuSpecInfo nuSpec in nuSpecs)
            {
                foreach (NuGetIdentifier dep in nuSpec.Dependencies ?? new NuGetIdentifier[0])
                {
                    NuSpecInfo fullNuSpec = nuSpecs.SingleOrDefault(x => x.ID == dep.ID);
                    if (fullNuSpec == null)
                        continue;

                    dep.UpdateVersionTo(fullNuSpec.VersionNumber);
                }
            }
        }
    }
}
