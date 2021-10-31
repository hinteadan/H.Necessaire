using H.Necessaire.CLI.Commands.NuGetVersioning.Models;
using System.Xml.Linq;

namespace H.Necessaire.CLI.Commands.NuGetVersioning
{
    class NuSpecFileUpdater
    {
        public async Task<OperationResult> SaveNuSpecs(params NuSpecInfo[] nuSpecs)
        {
            await
                Task.WhenAll(nuSpecs.Select(x => Task.Run(() => SaveNuSpec(x))));

            return OperationResult.Win();
        }

        private void SaveNuSpec(NuSpecInfo nuSpec)
        {
            XDocument nuSpecXml = XDocument.Parse(File.ReadAllText(nuSpec.FileInfo.FullName));

            XNamespace ns = nuSpecXml.Root?.GetDefaultNamespace() ?? XNamespace.None;

            XElement nuSpecMetadataElement = nuSpecXml.Root.Element(ns + "metadata");

            nuSpecMetadataElement.Element(ns + "version").Value = nuSpec.VersionNumber.ToString();

            if (nuSpec.Dependencies?.Any() ?? false)
            {
                foreach (XElement depElement in nuSpecMetadataElement.Element(ns + "dependencies").Elements(ns + "dependency"))
                {
                    string id = depElement.Attribute("id").Value;
                    depElement.Attribute("version").Value = nuSpec.Dependencies.Single(x => x.ID == id).VersionNumber.ToString();
                }
            }

            nuSpecXml.Save(nuSpec.FileInfo.FullName);
        }
    }
}
