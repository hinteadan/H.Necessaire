using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.UI.CLI.BLL
{
    internal class HNecessaireMauiRepoFinder
    {
        const string repoIdentifierFileName = "H.Necessaire.Runtime.MAUI.csproj";

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

        public OperationResult<DirectoryInfo> FindRootFolder()
        {
            DirectoryInfo fluentUiRepoDirectory = SearchByGoingUpFromCurrentDirectory();
            if (fluentUiRepoDirectory == null)
                return OperationResult<DirectoryInfo>.Fail("HNecessaireMauiRepo directory not found.").WithoutPayload<DirectoryInfo>();
            return fluentUiRepoDirectory.ToWinResult();
        }

        DirectoryInfo SearchByGoingUpFromCurrentDirectory()
        {
            DirectoryInfo currentDirectory = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));

            while (currentDirectory != null)
            {
                if (IsDirectoryFluentUiRepo(currentDirectory))
                    return currentDirectory;

                if (IsAnyDirectSubDirectoryFluentUiRepo(currentDirectory, out DirectoryInfo fluentUiRepoDirectory))
                    return fluentUiRepoDirectory;

                currentDirectory = currentDirectory.Parent;
            }

            return null;
        }

        bool IsAnyDirectSubDirectoryFluentUiRepo(DirectoryInfo directory, out DirectoryInfo fluentUiRepoDirectory)
        {
            fluentUiRepoDirectory = null;

            if (directory?.Exists != true)
                return false;

            fluentUiRepoDirectory = directory.GetDirectories().FirstOrDefault(IsDirectoryFluentUiRepo);

            return fluentUiRepoDirectory != null;
        }

        bool IsDirectoryFluentUiRepo(DirectoryInfo directory)
        {
            if (directory?.Exists != true)
                return false;

            return File.Exists(Path.Combine(directory.FullName, repoIdentifierFileName));
        }
    }
}
