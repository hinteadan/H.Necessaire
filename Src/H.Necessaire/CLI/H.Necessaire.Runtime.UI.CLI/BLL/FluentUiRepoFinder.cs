using System.Reflection;

namespace H.Necessaire.Runtime.UI.CLI.BLL
{
    internal class FluentUiRepoFinder
    {
        const string fluentUiRepoIdentifierFileName = "FluentIcons.podspec";

        public OperationResult<DirectoryInfo> Find()
        {
            DirectoryInfo fluentUiRepoDirectory = SearchByGoingUpFromCurrentDirectory();
            if (fluentUiRepoDirectory == null)
                return OperationResult<DirectoryInfo>.Fail("Fluent UI repo directory not found.").WithoutPayload<DirectoryInfo>();
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

            return File.Exists(Path.Combine(directory.FullName, fluentUiRepoIdentifierFileName));
        }
    }
}
