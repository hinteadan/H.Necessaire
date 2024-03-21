using H.Necessaire.CLI.Commands.HDoc.BLL;
using H.Necessaire.CLI.Commands.HDoc.Model;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.CLI.Commands.HDoc
{
    internal class HDocManager : ImADependency
    {
        HDocCsProjParser hDocCsProjParser;
        HDocCsFileParser hDocCsFileParser;
        ImAVersionProvider versionProvider;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            hDocCsProjParser = dependencyProvider.Get<HDocCsProjParser>();
            hDocCsFileParser = dependencyProvider.Get<HDocCsFileParser>();
            versionProvider = dependencyProvider.Get<ImAVersionProvider>();
        }

        public async Task<OperationResult<HDocumentation>> ParseDocumentation(DirectoryInfo srcFolder = null)
        {
            OperationResult<HDocProjectInfo[]> csProjsResult = hDocCsProjParser.Parse(srcFolder);
            if (!csProjsResult.IsSuccessful)
                return csProjsResult.WithoutPayload<HDocumentation>();
            HDocProjectInfo[] csProjs = csProjsResult.Payload;

            OperationResult<HDocTypeInfo[]>[] typesParseResults
                = (await Task.WhenAll(
                    csProjs.Select(async proj => await ParseTypesInProjects(proj))
                ))
                .SelectMany(x => x)
                .ToArray()
                ;

            HDocTypeInfo[] types
                = typesParseResults
                .Where(x => x.IsSuccessful)
                .SelectMany(x => x.Payload)
                .ToArray()
                ;



            return
                new HDocumentation
                {
                    AllTypes = types,
                    Version = await GetCurrentVersion(),
                }
                .ToWinResult()
                ;
        }

        private async Task<OperationResult<HDocTypeInfo[]>[]> ParseTypesInProjects(HDocProjectInfo projectInfo)
        {
            if (projectInfo?.CsFiles?.Any() != true)
                return Array.Empty<OperationResult<HDocTypeInfo[]>>();

            return
                await Task.WhenAll(
                    projectInfo.CsFiles.Select(async csFile => await hDocCsFileParser.Parse(csFile, projectInfo))
                );
        }

        private async Task<Version> GetCurrentVersion()
        {
            Version runtimeVersion = await versionProvider.GetCurrentVersion();
            if (runtimeVersion != Version.Unknown)
                return runtimeVersion;

            Versioning.Version version = H.Versioning.Version.Self.GetCurrent() ?? Versioning.Version.Unknown;

            return
                new Version
                {
                    Branch = version.Branch,
                    Commit = version.Commit,
                    Timestamp = version.Timestamp,
                    Number = new VersionNumber
                    {
                        Build = version.Number?.Build,
                        Major = version.Number?.Major ?? 0,
                        Minor = version.Number?.Minor ?? 0,
                        Patch = version.Number?.Patch,
                        Suffix = version.Number?.Suffix,
                    },
                };
        }
    }
}
