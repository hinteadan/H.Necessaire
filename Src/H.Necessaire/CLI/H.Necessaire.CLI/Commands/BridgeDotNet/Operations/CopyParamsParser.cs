using H.Necessaire.CLI.Commands.BridgeDotNet.Model;
using H.Necessaire.Runtime.CLI;
using System;
using System.IO;
using System.Linq;

namespace H.Necessaire.CLI.Commands.BridgeDotNet.Operations
{
    static class CopyParamsParser
    {
        public static OperationResult<BridgeDotNetCopyParams> ParseArgs(Note[] args, string[] usageSyntaxes)
        {
            OperationResult<BridgeDotNetCopyParams> result = FailWithUsageSyntax(usageSyntaxes).WithoutPayload<BridgeDotNetCopyParams>();

            if (!args?.Any() ?? true)
                return result;

            new Action(() =>
            {
                Note[] source = args?.Where(x => string.Equals(x.ID, "src", StringComparison.InvariantCultureIgnoreCase)).ToArray() ?? new Note[0];
                if (source.Length != 1)
                {
                    result
                        = OperationResult
                        .Fail("Copy params must contain exactly one Src argument", usageSyntaxes)
                        .WithoutPayload<BridgeDotNetCopyParams>();
                    return;
                }

                Note[] destinations = args?.Where(x => string.Equals(x.ID, "dst", StringComparison.InvariantCultureIgnoreCase)).ToArray() ?? new Note[0];
                if (!destinations.Any())
                {
                    result
                        = OperationResult
                        .Fail("Copy params must contain at least one Dst argument", usageSyntaxes)
                        .WithoutPayload<BridgeDotNetCopyParams>();
                    return;
                }

                BridgeDotNetCopyParams payload = new BridgeDotNetCopyParams
                {
                    SourceProjectRoot = new DirectoryInfo(source.Single().Value),
                    DestinationProjectsRoots = destinations.Select(x => new DirectoryInfo(x.Value)).ToArray(),
                };

                DirectoryInfo[] inexistentFolders = payload.SourceProjectRoot.AsArray().Concat(payload.DestinationProjectsRoots).Where(x => !x.Exists).ToArray();

                if (inexistentFolders.Any())
                {
                    result
                        = OperationResult
                        .Fail("The following folders do not exist", inexistentFolders.Select(x => x.FullName).ToArray())
                        .WithoutPayload<BridgeDotNetCopyParams>();
                    return;
                }

                result
                    = OperationResult
                    .Win()
                    .WithPayload(new BridgeDotNetCopyParams
                    {
                        SourceProjectRoot = new DirectoryInfo(source.Single().Value),
                        DestinationProjectsRoots = destinations.Select(x => new DirectoryInfo(x.Value)).ToArray(),
                    });
            })
            .TryOrFailWithGrace(
                onFail: x => result
                    = OperationResult
                    .Fail(x, "Cannot parse copy params from given arguments")
                    .And(r => r.Comments = usageSyntaxes)
                    .WithoutPayload<BridgeDotNetCopyParams>()
            );

            return result;
        }

        static OperationResult FailWithUsageSyntax(string[] usageSyntaxes)
        {
            return OperationResult.Fail(CLIPrinter.PrintUsageSyntax(usageSyntaxes));
        }
    }
}
