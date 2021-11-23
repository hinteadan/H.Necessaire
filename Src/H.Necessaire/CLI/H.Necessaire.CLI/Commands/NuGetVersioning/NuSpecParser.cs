using H.Necessaire.CLI.Commands.NuGetVersioning.Models;
using System.Runtime.Caching;
using System.Xml.Linq;

namespace H.Necessaire.CLI.Commands.NuGetVersioning
{
    class NuSpecParser : ImADependency
    {
        #region Construct
        const string cacheKeyNuSpecs = "NuSpecs";
        const string configKeyForNuSpectRootFolderPath = "NuSpectRootFolderPath";
        const string defaultNuSpectRootFolderPath = @"C:\H\H.Necessaire\Src\H.Necessaire";
        DirectoryInfo rootFolderToScan = new DirectoryInfo(defaultNuSpectRootFolderPath);
        RuntimeConfig runtimeConfig = new RuntimeConfig();
        readonly MemoryCache nuSpecsCache = new MemoryCache("NuSpecCache");
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            runtimeConfig = dependencyProvider.GetRuntimeConfig();
            rootFolderToScan = new DirectoryInfo(runtimeConfig.Get(configKeyForNuSpectRootFolderPath)?.Value?.ToString() ?? defaultNuSpectRootFolderPath);
        }
        #endregion

        public async Task<OperationResult<NuSpecInfo[]>> GetAllNuSpecs()
        {
            NuSpecInfo[]? nuSpecs = nuSpecsCache.Get(cacheKeyNuSpecs) as NuSpecInfo[];
            if (nuSpecs != null)
                return OperationResult.Win().WithPayload(nuSpecs);

            if (!rootFolderToScan.Exists)
                return OperationResult.Fail($"Folder {rootFolderToScan.FullName} doesn't exist").WithoutPayload<NuSpecInfo[]>();

            FileInfo[] nuSpecFiles = rootFolderToScan.GetFiles("*.nuspec", SearchOption.AllDirectories) ?? new FileInfo[0];

            if (!nuSpecFiles.Any())
                return OperationResult.Fail($"Folder {rootFolderToScan.FullName} and any subfolders don't contain any *.nuspec files").WithoutPayload<NuSpecInfo[]>();

            nuSpecs = (await Task.WhenAll(nuSpecFiles.Select(x => Task.Run(() => ParseNuSpecFile(x))))).ToArray() ?? new NuSpecInfo[0];

            nuSpecsCache.Add(cacheKeyNuSpecs, nuSpecs, DateTimeOffset.Now.AddMinutes(1));

            return OperationResult.Win().WithPayload(nuSpecs);
        }

        private NuSpecInfo ParseNuSpecFile(FileInfo fileInfo)
        {
            XDocument nuSpecXml = XDocument.Parse(File.ReadAllText(fileInfo.FullName));

            XNamespace ns = nuSpecXml.Root?.GetDefaultNamespace() ?? XNamespace.None;

            XElement? nuSpecMetadataElement = nuSpecXml.Root?.Element(ns + "metadata");

            return
                new NuSpecInfo(fileInfo)
                {
                    ID = nuSpecMetadataElement?.Element(ns + "id")?.Value ?? string.Empty,
                    Title = nuSpecMetadataElement?.Element(ns + "title")?.Value ?? string.Empty,
                    VersionNumber = ParseVersionNumber(nuSpecMetadataElement?.Element(ns + "version")?.Value),
                    Dependencies = ParseDependencies(nuSpecMetadataElement?.Element(ns + "dependencies"), ns),
                };
        }

        private NuGetIdentifier[] ParseDependencies(XElement? dependenciesElement, XNamespace ns)
        {
            IEnumerable<XElement> dependencyElements
                = dependenciesElement?.Elements(ns + "dependency")
                ?? Enumerable.Empty<XElement>();

            if (!dependencyElements.Any())
                return new NuGetIdentifier[0];

            return
                dependencyElements
                .Select(x => ParseDependency(x, ns))
                .ToArray();
        }

        private NuGetIdentifier ParseDependency(XElement dependencyElement, XNamespace ns)
        {
            return
                new NuGetIdentifier
                {
                    ID = dependencyElement.Attribute("id")?.Value ?? string.Empty,
                    VersionNumber = ParseVersionNumber(dependencyElement.Attribute("version")?.Value),
                }
                ;
        }

        private VersionNumber ParseVersionNumber(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return VersionNumber.Unknown;

            return VersionNumber.Parse(value);
        }
    }
}
