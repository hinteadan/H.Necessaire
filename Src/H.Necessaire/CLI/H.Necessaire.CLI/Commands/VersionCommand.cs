using H.Necessaire.Runtime.CLI.Commands;

namespace H.Necessaire.CLI.Commands
{
    internal class VersionCommand : CommandBase
    {
        public override Task<OperationResult> Run()
        {
            Console.WriteLine(GetCurrentVersion());

            return OperationResult.Win().AsTask();
        }

        private static Version GetCurrentVersion()
        {
            Versioning.Version version = H.Versioning.Version.Self.GetCurrent() ?? Versioning.Version.Unknown;

            return
                new Version
                {
                    Branch = version.Branch,
                    Commit = version.Commit,
                    Timestamp = version.Timestamp,
                    Number = new VersionNumber {
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
