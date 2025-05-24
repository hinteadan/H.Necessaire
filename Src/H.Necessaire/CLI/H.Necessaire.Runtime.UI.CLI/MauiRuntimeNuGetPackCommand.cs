using H.Necessaire.CLI.Commands;
using H.Necessaire.Runtime.ExternalCommandRunner;
using H.Necessaire.Runtime.UI.CLI.BLL;
using System.IO.Compression;

namespace H.Necessaire.Runtime.UI.CLI
{
    internal class MauiRuntimeNuGetPackCommand : CommandBase
    {
        HNecessaireMauiRepoFinder hNecessaireMauiRepoFinder;
        ImAContextualExternalCommandRunnerFactory contextualExternalCommandRunnerFactory;
        ImAnExternalCommandRunner externalCommandRunner;
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);

            hNecessaireMauiRepoFinder = dependencyProvider.Get<HNecessaireMauiRepoFinder>();
            externalCommandRunner = dependencyProvider.Get<ImAnExternalCommandRunner>();
            contextualExternalCommandRunnerFactory = dependencyProvider.Get<ImAContextualExternalCommandRunnerFactory>();
        }

        public override async Task<OperationResult> Run()
        {
            if (!hNecessaireMauiRepoFinder.Find().Ref(out var folderResult, out var folder))
                return folderResult;

            if (!(await externalCommandRunner.Run("dotnet", "pack", Path.Combine(folder.FullName, "H.Necessaire.Runtime.MAUI.csproj"))).RefResult(out var cmdResult))
                return cmdResult;

            if (!(await externalCommandRunner.Run("nuget", "pack", Path.Combine(folder.FullName, "H.Necessaire.Runtime.MAUI.nuspec"), "-OutputDirectory", Path.Combine(folder.FullName, "bin", "Release"))).RefResult(out cmdResult))
                return cmdResult;

            FileInfo libArchiveFile = new FileInfo(Path.Combine(folder.FullName, "bin", "Release", "H.Necessaire.Runtime.MAUI.1.0.0.nupkg"));
            FileInfo destinationArchiveFile = new DirectoryInfo(Path.Combine(folder.FullName, "bin", "Release")).EnumerateFiles("H.Necessaire.Runtime.MAUI.*.nupkg", SearchOption.TopDirectoryOnly).Single(f => f.Name != "H.Necessaire.Runtime.MAUI.1.0.0.nupkg");

            using (var libFileStream = libArchiveFile.OpenRead())
            using (var libArchive = new ZipArchive(libFileStream, ZipArchiveMode.Read))
                libArchive.ExtractToDirectory(Path.Combine(libArchiveFile.Directory.FullName, Path.GetFileNameWithoutExtension(libArchiveFile.FullName)));

            using (var destinationFileStream = destinationArchiveFile.OpenRead())
            using (var destinationArchive = new ZipArchive(destinationFileStream, ZipArchiveMode.Read))
                destinationArchive.ExtractToDirectory(Path.Combine(destinationArchiveFile.Directory.FullName, Path.GetFileNameWithoutExtension(destinationArchiveFile.FullName)));

            Directory.Move(
                Path.Combine(libArchiveFile.Directory.FullName, Path.GetFileNameWithoutExtension(libArchiveFile.FullName), "lib"),
                Path.Combine(destinationArchiveFile.Directory.FullName, Path.GetFileNameWithoutExtension(destinationArchiveFile.FullName), "lib")
            );


            //Copy DLLS to be included
            IEnumerable<DirectoryInfo> folders = new DirectoryInfo(Path.Combine(folder.FullName, "bin", "Release")).EnumerateDirectories("net*", SearchOption.TopDirectoryOnly);
            foreach(DirectoryInfo dir in folders)
            {
                FileInfo fileToCopy = dir.EnumerateFiles("H.Necessaire.Runtime.UI.dll", SearchOption.TopDirectoryOnly).Single();

                var folderToCopyTo = new DirectoryInfo(Path.Combine(destinationArchiveFile.Directory.FullName, Path.GetFileNameWithoutExtension(destinationArchiveFile.FullName), "lib")).EnumerateDirectories("net*", SearchOption.TopDirectoryOnly).Single(x => x.Name.StartsWith(fileToCopy.Directory.Name) || fileToCopy.Directory.Name.StartsWith(x.Name));

                File.Copy(fileToCopy.FullName, Path.Combine(folderToCopyTo.FullName, fileToCopy.Name));
            }
            //***


            File.Delete(libArchiveFile.FullName);
            File.Delete(destinationArchiveFile.FullName);
            Directory.Delete(Path.Combine(libArchiveFile.Directory.FullName, Path.GetFileNameWithoutExtension(libArchiveFile.FullName)), recursive: true);

            ZipFile.CreateFromDirectory(Path.Combine(destinationArchiveFile.Directory.FullName, Path.GetFileNameWithoutExtension(destinationArchiveFile.FullName)), destinationArchiveFile.FullName);

            Directory.Delete(Path.Combine(destinationArchiveFile.Directory.FullName, Path.GetFileNameWithoutExtension(destinationArchiveFile.FullName)), recursive: true);

            return OperationResult.Win();
        }
    }
}
