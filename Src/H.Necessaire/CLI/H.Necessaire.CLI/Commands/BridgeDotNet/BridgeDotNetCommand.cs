using H.Necessaire.CLI.Commands.BridgeDotNet.Model;
using H.Necessaire.Runtime.CLI.Commands;

namespace H.Necessaire.CLI.Commands.BridgeDotNet
{
    public class BridgeDotNetCommand : CommandBase
    {
        #region Construct
        static readonly string[] usageSyntaxes = new string[] {
            @"Syntax: BridgeDotNet copy ""Src=C:\H\H.Necessaire\Src\H.Necessaire\Apps\H.Necessaire.ReactAppSample"" ""Dst=C:\H\H.Necessaire\Src\H.Necessaire\Apps\H.Necessaire.AspNetCoreWebAppSample"" ""Dst=C:\H\H.Necessaire\Src\H.Necessaire\Apps\H.Necessaire.AspNetCoreWebAppSample2""",
        };
        static readonly string[] foldersToCopy = new string[] { "bridge", "AppIcon", "Refs", "Assets" };
        static readonly string[] foldersToMaintain = new string[] { "Assets" };
        const string destinationMainFolder = "Content";


        protected override string[] GetUsageSyntaxes() => usageSyntaxes;
        #endregion

        public override async Task<OperationResult> Run()
        {
            Note[] args = (await GetArguments())?.Jump(1) ?? new Note[0];

            if (!args.Any())
                return FailWithUsageSyntax();

            switch (args[0].Id.ToLowerInvariant())
            {
                case "copy": return await RunCopySubCommand(args.Jump(1));
                default: return FailWithUsageSyntax();
            }
        }

        private async Task<OperationResult> RunCopySubCommand(Note[] remainingArgs)
        {
            OperationResult<BridgeDotNetCopyParams> copyParamsParseResult
                = Operations.CopyParamsParser.ParseArgs(remainingArgs, usageSyntaxes) ?? FailWithUsageSyntax().WithoutPayload<BridgeDotNetCopyParams>();
            if (!copyParamsParseResult.IsSuccessful)
                return copyParamsParseResult;

            BridgeDotNetCopyParams copyParams = copyParamsParseResult.Payload;

            DirectoryInfo[] sourceFolders =
                foldersToCopy
                .SelectMany(folderToCopy =>
                    copyParams.SourceProjectRoot?.GetDirectories(folderToCopy, SearchOption.AllDirectories) ?? new DirectoryInfo[0]
                )
                .ToArray();

            FileInfo[] sourceFiles =
                sourceFolders
                .SelectMany(sourceFolder =>
                    sourceFolder.GetFiles(string.Empty, SearchOption.AllDirectories) ?? new FileInfo[0]
                )
                .ToArray();

            using (new TimeMeasurement(x => Log($"DONE Copying all BridgeDotNet files in {x}")))
            {
                await
                    Task.WhenAll(
                        copyParams.DestinationProjectsRoots.Select(destinationProjectRoot =>
                            Task.WhenAll(sourceFiles.Select(file => Task.Run(() => CopyBridgeNetFileToDestinationProject(file, destinationProjectRoot))))
                        )
                    );
            }

            return OperationResult.Win();
        }

        private void CopyBridgeNetFileToDestinationProject(FileInfo file, DirectoryInfo destinationProjectRoot)
        {
            bool isFileFolderMaintained = file.Directory?.Name.ToLowerInvariant().In(foldersToMaintain.Select(x => x.ToLowerInvariant())) ?? false;

            string destinationFolderPath = Path.Combine(destinationProjectRoot.FullName, destinationMainFolder, isFileFolderMaintained ? file.Directory?.Name ?? string.Empty : string.Empty) ?? string.Empty;

            string destinationFilePath = Path.Combine(destinationFolderPath, file.Name);

            Log($"Copying {file.FullName} TO {destinationFilePath}");

            using (new TimeMeasurement(x => Log($"DONE Copying {file.FullName} TO {destinationFilePath} in {x}")))
            {
                file.CopyTo(destinationFilePath, overwrite: true);
            }
        }
    }
}
