using System.Reflection;

namespace H.Necessaire.Runtime.UI.CLI.BLL.Abstracts
{
    abstract class RepoFinderBase
    {
        readonly string repoIdentifierFileName;

        protected RepoFinderBase(string repoIdentifierFileName)
        {
            this.repoIdentifierFileName = repoIdentifierFileName;
        }

        public OperationResult<DirectoryInfo> Find()
        {
            DirectoryInfo matchingDirectory = SearchByGoingUpFromCurrentDirectory();
            if (matchingDirectory == null)
                return OperationResult<DirectoryInfo>.Fail("Repo directory not found.").WithoutPayload<DirectoryInfo>();
            return matchingDirectory.ToWinResult();
        }

        DirectoryInfo SearchByGoingUpFromCurrentDirectory()
        {
            DirectoryInfo currentDirectory = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));

            while (currentDirectory != null)
            {
                if (IsDirectoryMatching(currentDirectory))
                    return currentDirectory;

                if (IsAnySubDirectoryMatching(currentDirectory, out DirectoryInfo matchingDirectory))
                    return matchingDirectory;

                currentDirectory = currentDirectory.Parent;
            }

            return null;
        }

        bool IsAnySubDirectoryMatching(DirectoryInfo directory, out DirectoryInfo matchingDirectory)
        {
            matchingDirectory = null;

            if (directory?.Exists != true)
                return false;

            DirectoryInfo[] directSubDirs = directory.GetDirectories();

            matchingDirectory = directSubDirs.FirstOrDefault(IsDirectoryMatching);

            if (matchingDirectory is null)
            {
                DirectoryInfo[] deepSubDirs = directory.GetDirectories("*", SearchOption.AllDirectories).ExceptBy(directSubDirs.Select(x => x.FullName), x => x.FullName).ToArray();

                matchingDirectory = deepSubDirs.FirstOrDefault(IsDirectoryMatching);
            }

            return !(matchingDirectory is null);
        }

        protected virtual bool IsDirectoryMatching(DirectoryInfo directory)
        {
            if (directory?.Exists != true)
                return false;

            return File.Exists(Path.Combine(directory.FullName, repoIdentifierFileName));
        }
    }
}
