using H.Necessaire.CLI.Commands.NuGetVersioning.Models;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace H.Necessaire.CLI.Commands.NuGetVersioning
{
    internal class CsprojParser : ImADependency
    {
        #region Construct
        const string cacheKeyCsprojs = "Csprojs";
        const string configKeyForNuSpecRootFolderPath = "NuSpecRootFolderPath";
        const string defaultNuSpecRootFolderPath = @"C:\H\H.Necessaire\Src\H.Necessaire";
        DirectoryInfo rootFolderToScan = new DirectoryInfo(defaultNuSpecRootFolderPath);
        RuntimeConfig runtimeConfig = RuntimeConfig.Empty;
        readonly MemoryCache csprojCache = new MemoryCache("CsprojCache");
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            runtimeConfig = dependencyProvider.GetRuntimeConfig();
            rootFolderToScan = new DirectoryInfo(runtimeConfig.Get(configKeyForNuSpecRootFolderPath)?.Value?.ToString() ?? defaultNuSpecRootFolderPath);
        }
        #endregion

        public Task<OperationResult<CsprojInfo[]>> GetAllCsProjs()
        {
            CsprojInfo[] csprojs = csprojCache.Get(cacheKeyCsprojs) as CsprojInfo[];
            if (csprojs != null)
                return OperationResult.Win().WithPayload(csprojs).AsTask();

            if (!rootFolderToScan.Exists)
                return OperationResult.Fail($"Folder {rootFolderToScan.FullName} doesn't exist").WithoutPayload<CsprojInfo[]>().AsTask();

            FileInfo[] csprojFiles = rootFolderToScan.GetFiles("*.csproj", SearchOption.AllDirectories) ?? new FileInfo[0];

            if (!csprojFiles.Any())
                return OperationResult.Fail($"Folder {rootFolderToScan.FullName} and any subfolders don't contain any *.csproj files").WithoutPayload<CsprojInfo[]>().AsTask();

            csprojs = csprojFiles.Select(x => ParseCsProjFile(x)).ToArray() ?? new CsprojInfo[0];
            //csprojs = (await Task.WhenAll(csprojFiles.Select(x => Task.Run(() => ParseCsProjFile(x))))).ToArray() ?? new CsprojInfo[0];

            csprojCache.Add(cacheKeyCsprojs, csprojs, DateTimeOffset.Now.AddMinutes(1));

            return OperationResult.Win().WithPayload(csprojs).AsTask();
        }

        private CsprojInfo ParseCsProjFile(FileInfo fileInfo)
        {
            XDocument csprojXml = XDocument.Parse(File.ReadAllText(fileInfo.FullName));

            XNamespace ns = csprojXml.Root?.GetDefaultNamespace() ?? XNamespace.None;

            XElement projectElement = csprojXml.Root;

            return
                new CsprojInfo(fileInfo)
                .And(x =>
                {
                    if (projectElement?.Attribute("Sdk")?.Value == null)
                        x = ParseNetFxCsProjFile(x, projectElement, ns);
                    else
                        x = ParseNetCsProjFile(x, projectElement, ns);
                });
        }

        private CsprojInfo ParseNetCsProjFile(CsprojInfo csprojInfo, XElement projectElement, XNamespace ns)
        {
            XElement targetFrameworkElement = projectElement?.Elements(ns + "PropertyGroup")?.Elements(ns + "TargetFramework")?.FirstOrDefault();

            return csprojInfo
                .And(x => x.IsNetFxFormat = false)
                .And(x =>
                {
                    x.Title = x.ID;
                    x.TargetFramework = targetFrameworkElement?.Value;
                    x.NuGets = ParseNuGetsFromNetCsProjFile(projectElement, ns);
                })
                ;
        }

        private CsprojInfo ParseNetFxCsProjFile(CsprojInfo csprojInfo, XElement projectElement, XNamespace ns)
        {
            XElement assemblyNameElement = projectElement?.Elements(ns + "PropertyGroup")?.Elements(ns + "AssemblyName")?.FirstOrDefault();
            XElement targetFrameworkVersionElement = projectElement?.Elements(ns + "PropertyGroup")?.Elements(ns + "TargetFrameworkVersion")?.FirstOrDefault();

            return csprojInfo
                .And(x => x.IsNetFxFormat = true)
                .And(x =>
                {
                    x.Title = assemblyNameElement?.Value ?? x.ID;
                    x.TargetFramework = $".NET Framework {targetFrameworkVersionElement?.Value}".Trim();
                    x.NuGets = ParseNuGetsFromNetFxCsProjFile(projectElement, ns);
                })
                ;
        }

        private NuGetIdentifier[] ParseNuGetsFromNetCsProjFile(XElement projectElement, XNamespace ns)
        {
            XElement[] packageReferenceElements = projectElement?.Elements(ns + "ItemGroup")?.Elements(ns + "PackageReference")?.ToArray();
            return
                packageReferenceElements
                ?.Select(x => ParseNetPackageReferenceElement(x, ns))
                ?.ToArray()
                ??
                new NuGetIdentifier[0]
                ;
        }

        private NuGetIdentifier[] ParseNuGetsFromNetFxCsProjFile(XElement projectElement, XNamespace ns)
        {
            XElement[] packageReferenceElements = projectElement?.Elements(ns + "ItemGroup")?.Elements(ns + "PackageReference")?.ToArray();
            return
                packageReferenceElements
                ?.Select(x => ParseNetFxPackageReferenceElement(x, ns))
                ?.ToArray()
                ??
                new NuGetIdentifier[0]
                ;
        }

        private NuGetIdentifier ParseNetPackageReferenceElement(XElement packageReferenceElement, XNamespace ns)
        {
            return
                new NuGetIdentifier
                {
                    ID = packageReferenceElement?.Attribute("Include")?.Value,
                    VersionNumber = ParseVersionNumber(packageReferenceElement?.Attribute("Version")?.Value),
                }
                ;
        }

        private NuGetIdentifier ParseNetFxPackageReferenceElement(XElement packageReferenceElement, XNamespace ns)
        {
            return
                new NuGetIdentifier
                {
                    ID = packageReferenceElement?.Attribute("Include")?.Value,
                    VersionNumber = ParseVersionNumber(packageReferenceElement?.Element(ns + "Version")?.Value),
                }
                ;
        }

        private VersionNumber ParseVersionNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return VersionNumber.Unknown;

            return VersionNumber.Parse(value);
        }
    }
}
