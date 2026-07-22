using System;
using System.IO;
using System.Threading.Tasks;

namespace H.Necessaire.CLI.Commands
{
    internal class VersionCommand : CommandBase
    {
        static readonly string[] usageSyntax = new string[] {
            "version [out=path/to/version.txt]",
        };
        protected override string[] GetUsageSyntaxes() => usageSyntax;

        public override async Task<OperationResult> Run()
        {
            Console.WriteLine(GetCurrentVersion().RefTo(out Version version));

            string outputPath = (await GetArguments()).Get("out", ignoreCase: true);

            if (!outputPath.IsEmpty())
            {
                return HSafe.Run(() => File.WriteAllText(outputPath, version.ToString()));
            }

            return OperationResult.Win();
        }

        private static Version GetCurrentVersion()
        {
            Versioning.Version version = H.Versioning.Version.Self.GetCurrent() ?? Versioning.Version.Unknown;

            return version.ToHsVersion();
        }
    }
}
