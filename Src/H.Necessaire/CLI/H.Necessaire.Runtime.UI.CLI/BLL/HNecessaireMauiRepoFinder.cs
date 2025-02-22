using H.Necessaire.Runtime.UI.CLI.BLL.Abstracts;

namespace H.Necessaire.Runtime.UI.CLI.BLL
{
    internal class HNecessaireMauiRepoFinder : RepoFinderBase
    {
        public HNecessaireMauiRepoFinder() : base("H.Necessaire.Runtime.MAUI.csproj") { }

        public OperationResult<DirectoryInfo> FindWellKnownFluentUiGlyphsFolder()
        {
            OperationResult<DirectoryInfo> rootFolderResult = FindRootFolder();
            if (!rootFolderResult.IsSuccessful)
                return rootFolderResult;

            DirectoryInfo result
                = new DirectoryInfo(
                    Path.Combine(
                        rootFolderResult.Payload.FullName,
                        "WellKnown",
                        "FluentUI",
                        "Glyphs"
                    )
                );

            if (!result.Exists)
                return OperationResult.Fail("Glyphs folder not found").WithPayload(result);

            return result.ToWinResult();
        }

        public OperationResult<DirectoryInfo> FindRootFolder() => Find();
    }
}
