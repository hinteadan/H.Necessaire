using H.Necessaire.CLI.Commands.NuGetVersioning.Models;
using H.Necessaire.Runtime.CLI.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.CLI.Commands.NuGetVersioning
{
    public class NuGetVersionCommand : CommandBase
    {
        #region Construct
        static readonly string usageSyntax = "Syntax: update [RavenDB.Client=5.2.5] [H.Necessaire=major|minor|patch] [...]" +
            $"{Environment.NewLine}--OR--{Environment.NewLine}" +
            $"        consolidate-deps";

        NuSpecVersionProcessor nuSpecVersionProcessor = null;
        CsprojNugetVersionProcessor csprojNugetVersionProcessor = null;
        NuSpecFileUpdater nuSpecFileUpdater = null;
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);
            nuSpecVersionProcessor = dependencyProvider.Get<NuSpecVersionProcessor>();
            csprojNugetVersionProcessor = dependencyProvider.Get<CsprojNugetVersionProcessor>();
            nuSpecFileUpdater = dependencyProvider.Get<NuSpecFileUpdater>();
        }

        protected override string[] GetUsageSyntaxes() => usageSyntax.AsArray();
        #endregion

        public override async Task<OperationResult> Run()
        {
            Note[] args = (await GetArguments())?.Jump(1) ?? new Note[0];

            if (!args.Any())
                return FailWithUsageSyntax();

            switch (args[0].ID.ToLowerInvariant())
            {
                case "update": return await RunUpdateSubCommand(args.Jump(1));
                case "consolidate-deps": return await RunConsolidateSubCommand(args.Jump(1));
                default: return OperationResult.Fail(usageSyntax);
            }
        }

        private async Task<OperationResult> RunConsolidateSubCommand(Note[] args)
        {
            NuGetIdentifier[] nuGetsFromCsProjs = (await csprojNugetVersionProcessor.GetAllLatestNuGets()).ThrowOnFailOrReturn();

            foreach (NuGetIdentifier externalNuget in nuGetsFromCsProjs)
            {
                NuSpecInfo[] nuSpecs = (await nuSpecVersionProcessor.UpdateExternalNuGetVersion(externalNuget.ID, externalNuget.VersionNumber.ToString(), updateNuSpecVersion: false)).ThrowOnFailOrReturn();

                await nuSpecFileUpdater.SaveNuSpecs(nuSpecs);
            }

            return OperationResult.Win();
        }

        private async Task<OperationResult<NuSpecInfo[]>> RunUpdateSubCommand(Note[] args)
        {
            NuSpecInfo[] nuSpecs = new NuSpecInfo[0];
            foreach (Note arg in args)
            {
                string name = arg.ID;
                string version = arg.Value;

                bool isOwnNuGet = name.StartsWith("H.Necessaire", StringComparison.InvariantCultureIgnoreCase);

                if (!isOwnNuGet)
                {
                    nuSpecs = (await nuSpecVersionProcessor.UpdateExternalNuGetVersion(name, version)).ThrowOnFailOrReturn();
                }
                else
                {
                    switch (version.ToLowerInvariant())
                    {
                        case "major":
                            nuSpecs = (await nuSpecVersionProcessor.IncrementMajorVersion(name)).ThrowOnFailOrReturn();
                            break;
                        case "minor":
                            nuSpecs = (await nuSpecVersionProcessor.IncrementMinorVersion(name)).ThrowOnFailOrReturn();
                            break;
                        case "patch":
                        default:
                            nuSpecs = (await nuSpecVersionProcessor.IncrementPatchVersion(name)).ThrowOnFailOrReturn();
                            break;
                    }
                }
            }

            NuSpecInfo[] dirtyNuSpecs = nuSpecs.Where(x => x.IsDirtyVersion).ToArray();

            if (!dirtyNuSpecs.Any())
                return OperationResult.Win().WithPayload(nuSpecs);

            await nuSpecFileUpdater.SaveNuSpecs(dirtyNuSpecs);

            return OperationResult.Win().WithPayload(nuSpecs);
        }
    }
}
