using H.Necessaire.CLI.Commands.NuGetVersioning.Models;
using H.Necessaire.Runtime.CLI.Commands;

namespace H.Necessaire.CLI.Commands.NuGetVersioning
{
    public class NuGetVersionCommand : CommandBase
    {
        #region Construct
        const string usageSyntax = "Syntax: update [RavenDB.Client=5.2.5] [H.Necessaire=major|minor|patch] [...]";

        NuSpecVersionProcessor nuSpecVersionProcessor = new NuSpecVersionProcessor();
        NuSpecFileUpdater nuSpecFileUpdater = new NuSpecFileUpdater();
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);
            nuSpecVersionProcessor = dependencyProvider.Get<NuSpecVersionProcessor>();
            nuSpecFileUpdater = dependencyProvider.Get<NuSpecFileUpdater>();
        }
        #endregion

        public override async Task<OperationResult> Run()
        {
            Note[] args = (await GetArguments())?.Jump(1) ?? new Note[0];

            if (!args.Any())
                return OperationResult.Fail(usageSyntax);

            switch (args[0].Id.ToLowerInvariant())
            {
                case "update": return await RunUpdateSubCommand(args.Jump(1));
                default: return OperationResult.Fail(usageSyntax);
            }
        }

        private async Task<OperationResult<NuSpecInfo[]>> RunUpdateSubCommand(Note[] args)
        {
            NuSpecInfo[] nuSpecs = new NuSpecInfo[0];
            foreach (Note arg in args)
            {
                string name = arg.Id;
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
